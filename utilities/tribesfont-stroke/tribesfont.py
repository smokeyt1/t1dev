import helper
import struct
import math
import numpy as np
# Requires
# https://github.com/python-pillow/Pillow
from PIL import Image, ImageDraw, ImageFont, ImageColor

# import tribesfont_pal

DEBUG_EXPORT = False

def clamp_byte(num):
    return max(min(num, 255), 0)

class TribesFont:
    def __init__(self):
        self.font_info: GFXFontInfo = None
        self.char_table_lookup_size = None
        self.char_table_lookup_offset = None
        self.char_table_lookup = []
        self.char_table: GFXCharInfo = []
        return

    def load_file(self, file_name):
        with open(file_name, "rb") as file:
            data = file.read()
            return self.load_binary(data)

    def load_binary(self, data):
        if data[:4] != b"PFON":
            print("Wrong PFON header")
            return

        curr_data_index = [4]
        chunk_size = helper.get_int(data, curr_data_index)
        curr_data_index[0] += 4  # Flags?  Don't know...skipping for now

        self.font_info = GFXFontInfo()
        self.font_info.read(data, curr_data_index)

        self.char_table_lookup_size = helper.get_int16(data, curr_data_index)
        self.char_table_lookup_offset = helper.get_int16(data, curr_data_index)

        self.char_table_lookup = []
        for i in range(self.char_table_lookup_size):
            self.char_table_lookup.append(helper.get_int16(data, curr_data_index))

        self.char_table = []
        for i in range(self.font_info.num_chars):
            char = GFXCharInfo()
            char.read(data, curr_data_index)
            self.char_table.append(char)

        #bitmap array

        # pal attached
        # pal id
        return

    def print_stats(self):
        self.font_info.print_stats()
        print(f"char_table_lookup_size - {self.char_table_lookup_size}")
        print(f"char_table_lookup_offset - {self.char_table_lookup_offset}")

        for ctl in self.char_table_lookup:
            print(f"char_table_lookup - {ctl}")

        for i in range(len(self.char_table)):
            print(f"-------------------")
            print(f"Char table num {i}")
            self.char_table[i].print_stats()

    def save_pft(self, export_file_name, width, height, bmp_count):
        data = b'PFON'

        bdata = self.fake_pbma(width, height, bmp_count)
        psize = len(bdata)
        data += struct.pack("I", psize + 52 + self.char_table_lookup_size * 2 + self.font_info.num_chars * 8)
        data += struct.pack("I", 2)
        data += struct.pack("IIiiiiiiiii",
                            self.font_info.flags,
                            self.font_info.justification,
                            self.font_info.num_chars,
                            int(self.font_info.font_height),
                            int(self.font_info.font_width),
                            self.font_info.fore_color,
                            self.font_info.back_color,
                            int(self.font_info.baseline),
                            self.font_info.scale_x,
                            self.font_info.scale_y,
                            self.font_info.spacing)

        data += struct.pack("hh",
                            self.char_table_lookup_size,
                            self.char_table_lookup_offset)

        for i in range(self.char_table_lookup_size):
            data += struct.pack("h", self.char_table_lookup[i])

        for i in range(self.font_info.num_chars):
            data += struct.pack("BBBBBbBB",
                                clamp_byte(self.char_table[i].bitmap_no),
                                int(clamp_byte(self.char_table[i].x)),
                                int(clamp_byte(self.char_table[i].y)),
                                int(clamp_byte(self.char_table[i].width)),
                                int(clamp_byte(self.char_table[i].height)),
                                int(self.char_table[i].baseline),
                                clamp_byte(self.char_table[i].padding_0),
                                clamp_byte(self.char_table[i].padding_1))

        data += bdata

        data += struct.pack("I", 0)

        file = open(export_file_name, "wb")
        file.write(data)
        file.close()

        return

    def fake_pbma(self, width, height, count):
        # We don't care about rmap or user
        data = b'PBMA'
        # Need to figure out the size info
        bdata = self.fake_pbmp(width, height)

        data += struct.pack("I", len(bdata) * count)  # This actually isn't used, but lets make it the size
        data += b'head'
        data += struct.pack("I", 8)  # 2 ints in the size
        data += struct.pack("I", 5)  # version
        data += struct.pack("I", count)  # count

        for _ in range(count):
            data += bdata

        return data


    def fake_pbmp(self, width, height):
        data = b'PBMP'
        #size
        data += struct.pack("I", (width * height) + ((width * 8 >> 3) + 3) & ~3 + 32)
        data += b'head'
        data += struct.pack("I", 20)  # Size
        data += struct.pack("I", 2)  # Version
        data += struct.pack("I", width)
        data += struct.pack("I", height)
        data += struct.pack("I", 8) # bitdepth
        data += struct.pack("I", 9) # attribute
        data += b'data'
        data += struct.pack("I", (width * height) + ((width * 8 >> 3) + 3) & ~3)
        data += bytearray(width*height)  #haha fake data inbound!
        data += b'DETL'
        data += struct.pack("I", 4)
        data += b'PiDX'
        data += struct.pack("I", 1136) # A pal that exists
        return data

def furthest_from_zero(number):
    if number < 0:
        return math.floor(number)
    return math.ceil(number)

def blend_ttf_font(fonts, offsets, alias_modes=None):
    resulting_font = TTFFont()

    # Build everything one character at a time
    for char_index in range(256):
        max_left = 0
        max_right = 0
        max_top = 0
        max_bottom = 0
        for font_index in range(len(fonts)):
            font = fonts[font_index]
            left, top, right, bottom = font.char_bounds[char_index]
            width = right - left
            height = bottom - top
            left = furthest_from_zero(-width/2 + offsets[font_index][char_index][0])
            bottom = furthest_from_zero(height/2 + offsets[font_index][char_index][1])
            right = furthest_from_zero(width/2 + offsets[font_index][char_index][0])
            top = furthest_from_zero(-height/2 + offsets[font_index][char_index][1])

            # Left is "negative"
            if max_left > left:
                max_left = left
            if max_right < right:
                max_right = right
            if max_top > top:
                max_top = top
            if max_bottom < bottom:
                max_bottom = bottom


        # At this point we know we have the extremes, so we can make an image with the size needed
        # print(f"Width {chr(char_index)} {max_right - max_left} {max_bottom - max_top}")
        img_bbox = (int(max(1, max_right - max_left + 1)), int(max(1, max_bottom - max_top + 1)))
        np_img = np.zeros((img_bbox[1], img_bbox[0], 4), np.ubyte)
        img_center = (img_bbox[0] / 2, img_bbox[1] / 2)
        for font_index in range(len(fonts)):
            font = fonts[font_index]

            center = ((font.char_bounds[char_index][2] - font.char_bounds[char_index][0]) / 2,
                      (font.char_bounds[char_index][3] - font.char_bounds[char_index][1]) / 2)

            left, top = (int(offsets[font_index][char_index][0] - max_left - center[0]),  # x
                       int(offsets[font_index][char_index][1] - max_top - center[1]))
            char_image = np.array(font.char_images[char_index])
            height, width, channels = char_image.shape

            if alias_modes is not None:
                alias_mode = alias_modes[font_index]
            else:
                alias_mode = None
            if height > 1 and width > 1:
                for y in range(height):
                    for x in range(width):
                        if img_bbox[1] <= top+y:
                            continue
                        if img_bbox[0] <= left+x:
                            continue
                        sr, sg, sb, sa = np_img[top + y, left + x]
                        cr, cg, cb, ca = char_image[y, x]
                        if ca == 0:
                            continue

                        if alias_mode == "Thinly":
                            ca = (ca ** 3) // (255 ** 2)
                        elif alias_mode == "Thickly":
                            ca = 255 - (((255 - ca) ** 3) // (255 ** 2))
                        elif alias_mode == "Threshold":
                            ca = ca if ca > 128 else 0
                        elif alias_mode == "Threshold1.11":
                            ca = 255 if ca > 128 else 0

                        if ca == 255 or sa == 0: # Because it's just easier this way
                            np_img[top + y, left + x, 0] = int(cr)
                            np_img[top + y, left + x, 1] = int(cg)
                            np_img[top + y, left + x, 2] = int(cb)
                            np_img[top + y, left + x, 3] = int(ca)
                            continue


                        ca_scale = ca / 255
                        sa_scale = (255 - ca) / 255
                        total_alpha = (sa * sa_scale) + (ca * ca_scale)
                        ca_factor = ca * ca_scale / total_alpha
                        sa_factor = sa * sa_scale / total_alpha

                        r = int(cr) * ca_factor + int(sr) * sa_factor
                        g = int(cg) * ca_factor + int(sg) * sa_factor
                        b = int(cb) * ca_factor + int(sb) * sa_factor
                        a = max(ca, sa)
                        np_img[top + y, left + x, 0] = int(r)
                        np_img[top + y, left + x, 1] = int(g)
                        np_img[top + y, left + x, 2] = int(b)
                        np_img[top + y, left + x, 3] = int(a)

        img = Image.fromarray(np_img)
        font = fonts[-1]
        center = ((font.char_bounds[char_index][2] - font.char_bounds[char_index][0]) / 2,
            (font.char_bounds[char_index][3] - font.char_bounds[char_index][1]) / 2)
        offset = (int(offsets[-1][char_index][0] - max_left - center[0]),  # x
                       int(offsets[-1][char_index][1] -max_top - center[1]))

        #offset is now essentially the top of the image
        baseline = offset[1] + font.char_baseline[char_index]
        height = max_bottom - max_top
        #draw = ImageDraw.Draw(img)
        #draw.line((0, baseline, int(max(1, max_bottom - max_top)), baseline), fill="Blue")

        #print(chr(char_index))
        #print((0, 0, max_right - max_left, max_bottom - max_top))
        resulting_font.char_baseline[char_index] = baseline
        resulting_font.char_images[char_index] = img
        resulting_font.char_bounds[char_index] = (0, 0, max_right - max_left, height)
        resulting_font.metrics[char_index] = (offset[1] + font.metrics[char_index][0], font.metrics[char_index][1])
        if DEBUG_EXPORT:
            png_export_name = f".\\debug\\{char_index}b.png"
            img.save(png_export_name)

    return resulting_font

class TTFFont:
    def __init__(self):
        self.char_images = [None]*256
        self.metrics = [None]*256
        self.char_bounds = [None]*256
        self.char_baseline = [None]*256

    def load_font_all(self, font, fill):
        for i in range(256):
            self.load_font(font, fill, i)

    def load_font(self, font, fill, char_index, stroke_width, stroke_color):
        self.metrics[char_index] = font.getmetrics()
        char = chr(char_index)
        bounds = font.getbbox(char)
        bounds = (bounds[0] - stroke_width,
                  bounds[1] - stroke_width,
                  bounds[2] + stroke_width,
                  bounds[3] + stroke_width)
        # left, top, right, bottom = bounds
        # print(f"{chr(char_index)} {left} {right} {top} {bottom}")
        img = Image.new("RGBA", (max(1, bounds[2] - bounds[0]), max(1, bounds[3] - bounds[1])), "#00000000")
        draw = ImageDraw.Draw(img)

        self.char_bounds[char_index] = bounds
        self.char_baseline[char_index] = -bounds[1] + self.metrics[char_index][0]
        draw.text((-bounds[0], -bounds[1]), char, fill=fill, font=font,
                  stroke_width=stroke_width, stroke_fill=stroke_color)

        if DEBUG_EXPORT:
            png_export_name = f".\\debug\\{char_index}.png"
            img.save(png_export_name)

        self.char_images[char_index] = img
        #char_index = 87
        #left, top, right, bottom = self.char_bounds[char_index]
        #print(f"{chr(char_index)} {left} {right} {top} {bottom}")

    def to_large_image(self):
        left, top, right, bottom = self.max_bounds()
        max_width = int(right - left)
        max_height = int(bottom - top)
        png_image = Image.new("RGBA", (16 * max_width, 16 * max_height), "#00000000")
        pos = [0, 0]
        for column_index in range(16):
            for row_index in range(16):
                char_index = row_index + column_index * 16
                png_image.paste(self.char_images[char_index], (int(pos[0]), int(pos[1])))

                pos[0] += max_width
            pos[0] = 0
            pos[1] += max_height

        return png_image

    def max_bounds(self):
        max_left = 1000
        max_right = 0
        max_bottom = 0
        max_top = 1000
        for left, top, right, bottom in self.char_bounds:
            if max_left > left:
                 max_left = left
            if max_bottom < bottom:
                max_bottom = bottom
            if max_right < right:
                max_right = right
            if max_top > top:
                max_top = top

        return max_left, max_top, max_right, max_bottom


def find_true_char_width(img):
    char_image = np.asarray(img)
    height, width, channels = char_image.shape

    min_pos = 10000
    max_pos = 0
    for y in range(height):
        for x in range(width):
            alpha = char_image[y, x, 3]
            if alpha < 10:
                continue

            if x < min_pos:
                min_pos = x
            if x > max_pos:
                max_pos = x

    return min_pos, max_pos+1

def export_from_ttf(export_folder, export_file_name, ttf_font: TTFFont, spacing=1, baseline_offset=0, space_char_width=None, width_height_offsets=[(0,0)]*256):
    if export_folder[-1] != '/' and export_folder[-1] != '\\':
        export_folder += "/"
    full_text_range = ""
    for char_index in range(1,256):
        full_text_range += chr(char_index)

    tfont = TribesFont()
    tfont.font_info = GFXFontInfo()
    tfont.char_table_lookup_size = 256
    tfont.char_table_lookup_offset = 0  # Ours will have all characters
    for i in range(256):
        tfont.char_table_lookup.append(i)

    for i in range(256):
        tfont.char_table.append(GFXCharInfo())

    descent, ascent = ttf_font.metrics[char_index]

    m_left, m_top, m_right, m_bottom = ttf_font.max_bounds()

    tfont.font_info.flags = 69  # This pretty much is static, Tribes doesn't support the TrueType flag
    tfont.font_info.justification = 0
    tfont.font_info.num_chars = 256
    tfont.font_info.font_height = m_bottom - m_top  # Used on fixed width
    tfont.font_info.font_width = m_right - m_left  # Used on fixed width
    tfont.font_info.fore_color = 0  # color for transparent
    tfont.font_info.back_color = 0
    tfont.font_info.baseline = descent - ascent + baseline_offset
    tfont.font_info.scale_x = int(1 << 16)
    tfont.font_info.scale_y = int(1 << 16)
    tfont.font_info.spacing = spacing

    # We are limited to 256 x 256 sized images...so now we need to break it all apart
    final_width, final_height = (256, 256)
    png_image = Image.new("RGBA", (final_width, final_height), "#00000000")
    draw = ImageDraw.Draw(png_image)

    bitmap_no = 0
    pos = [0, 0]
    inc_pos = [0, 0]
    for char_index in range(256):
        if char_index == 0:
            char = chr(1)
        else:
            char = chr(char_index)

        left, top, right, bottom = ttf_font.char_bounds[char_index]
        inc_pos[0] = right - left
        if bottom - top > inc_pos[1]:
            inc_pos[1] = bottom - top

        if char_index != 32:
            true_left, true_right = find_true_char_width(ttf_font.char_images[char_index])
        elif space_char_width is None or space_char_width <= 0:
            true_left = left
            true_right = right
        else:
            true_left = left + space_char_width

        height = bottom - top - width_height_offsets[char_index][1]
        tfont.char_table[char_index].bitmap_no = bitmap_no
        tfont.char_table[char_index].x = pos[0] + true_left
        tfont.char_table[char_index].y = pos[1]
        tfont.char_table[char_index].width = true_right - true_left - width_height_offsets[char_index][0]
        tfont.char_table[char_index].height = height
        tfont.char_table[char_index].baseline = height - ttf_font.char_baseline[char_index]
        tfont.char_table[char_index].padding_0 = 0
        tfont.char_table[char_index].padding_1 = 0

        png_image.paste(ttf_font.char_images[char_index], (int(pos[0]), int(pos[1])))

        pos[0] += int(inc_pos[0])

        next_width = 0
        next_height = 0
        if char_index < 255:
            next_width = ttf_font.char_bounds[char_index + 1][2] - ttf_font.char_bounds[char_index + 1][0]
            next_height = ttf_font.char_bounds[char_index + 1][3] - ttf_font.char_bounds[char_index + 1][1]

        if pos[0] + next_width >= 256:  # not enough room on this line.  Put it on the next line
            pos[0] = 0
            pos[1] += int(inc_pos[1])
            inc_pos[1] = 0
        if pos[1] + next_height >= 256:  # not enough room need to move to the next bitmap
            png_export_name = f"{export_folder}{export_file_name}.pft.{str(bitmap_no).zfill(3)}.png"
            png_image.save(png_export_name, subsampling=0, quality=100)
            png_image = Image.new("RGBA", (final_width, final_height), "#00000000")
            draw = ImageDraw.Draw(png_image)

            bitmap_no += 1
            pos = [0, 0]
            inc_pos = [0, 0]

    if pos[0] != 0 and pos[1] != 0:
        png_export_name = f"{export_folder}{export_file_name}.pft.{str(bitmap_no).zfill(3)}.png"
        png_image.save(png_export_name, subsampling=0, quality=100)
        # tfp = tribesfont_pal.Tribes_Palette('pal\\3.pal')
        # tfp.convert_img_to_data(png_image)
    else:
        bitmap_no -= 1

    pft_export_name = f"{export_folder}{export_file_name}.pft"
    tfont.save_pft(pft_export_name, 256, 256, bitmap_no + 1)


class GFXFontInfo:
    def __init__(self):
        self.flags = None  # UINT32
        self.justification = None  # UINT32
        self.num_chars = None  # Int32
        self.font_height = None  # Int32
        self.font_width = None  # Int32
        self.fore_color = None  # Int32
        self.back_color = None  # Int32
        self.baseline = None  # Int32
        self.scale_x = None
        self.scale_y = None
        self.spacing = None  # Int32
        return

    def read(self, data, offset):
        self.flags = helper.get_uint(data, offset)
        self.justification = helper.get_uint(data, offset)
        self.num_chars = helper.get_int(data, offset)
        self.font_height = helper.get_int(data, offset)
        self.font_width = helper.get_int(data, offset)
        self.fore_color = helper.get_int(data, offset)
        self.back_color = helper.get_int(data, offset)
        self.baseline = helper.get_int(data, offset)
        self.scale_x = helper.get_int(data, offset)
        self.scale_y = helper.get_int(data, offset)
        self.spacing = helper.get_int(data, offset)
        return

    def print_stats(self):
        print(f"flags - {self.flags}")
        print(f"justification - {self.justification}")
        print(f"num_chars - {self.num_chars}")
        print(f"font_height - {self.font_height}")
        print(f"font_width - {self.font_width}")
        print(f"fore_color - {self.fore_color}")
        print(f"back_color - {self.back_color}")
        print(f"baseline - {self.baseline}")
        print(f"scale_x - {self.scale_x}")
        print(f"scale_y - {self.scale_y}")
        print(f"spacing - {self.spacing}")

class GFXCharInfo:
    def __init__(self):
        self.bitmap_no = None  # uint8
        self.x = None  # uint8
        self.y = None  # uint8
        self.width = None  # uint8
        self.height = None  # uint8
        self.baseline = None  # int8
        self.padding_0 = None  # uint8
        self.padding_1 = None   # uint8
        return

    def read(self, data, offset):
        self.bitmap_no = helper.get_uint8(data, offset)
        self.x = helper.get_uint8(data, offset)  # uint8
        self.y = helper.get_uint8(data, offset)  # uint8
        self.width = helper.get_uint8(data, offset)  # uint8
        self.height = helper.get_uint8(data, offset)  # uint8
        self.baseline = helper.get_int8(data, offset)  # int8
        self.padding_0 = helper.get_uint8(data, offset)  # uint8
        self.padding_1 = helper.get_uint8(data, offset)   # uint8
        return

    def print_stats(self):
        print(f"bitmapNo - {self.bitmap_no}")
        print(f"x - {self.x}")
        print(f"y - {self.y}")
        print(f"width - {self.width}")
        print(f"height - {self.height}")
        print(f"baseline - {self.baseline}")
        print(f"padding_0 - {self.padding_0}")
        print(f"padding_1 - {self.padding_1}")
        return

class FontFlags:
   FONT_PROPORTIONAL = 1<<0
   FONT_FIXED        = 1<<1
   FONT_MONO         = 1<<2
   FONT_BITMAP       = 1<<3
   FONT_TRUETYPE     = 1<<4
   FONT_RASTERIZED   = 1<<5
   FONT_TRANSPARENT  = 1<<6
   FONT_TRANSLUCENT  = 1<<7
   FONT_UNDERLINED   = 1<<8
   FONT_UNICODE      = 1<<9
   FONT_LOWERCAPS    = 1<<10
