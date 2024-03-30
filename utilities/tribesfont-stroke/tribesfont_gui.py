import math

import tribesfont
import tkinter as tk
from tkinter import ttk
from tkinter import filedialog as fd
from tkinter import colorchooser as cc
from tkinter.messagebox import askyesno
from tkinter import messagebox
import os
from PIL import Image, ImageDraw, ImageFont, ImageColor, ImageTk
all_picture_labels = []
output_font = None

class GuiLayerInformation():
    def __init__(self):
        self.font_location = None
        self.font_size = 18
        self.stroke_width = 0
        self.stroke_color = "Black"
        self.char_colors = ["White"]*256
        self.fill_trans = False
        self.stroke_trans = False
        self.offset = [0, 0]
        self.char_offsets = [[0, 0]]*256
        self.aliasing = "Default"
        return

    def copy(self):
        result = GuiLayerInformation()
        result.font_location = self.font_location
        result.font_size = self.font_size
        result.stroke_width = self.stroke_width
        result.stroke_color = self.stroke_color
        result.char_colors = self.char_colors.copy()
        result.fill_trans = self.fill_trans
        result.stroke_trans = self.stroke_trans
        result.offset = self.offset.copy()
        result.char_offsets = self.char_offsets.copy()
        return result

    def get_font(self):
        font = ImageFont.truetype(self.font_location, self.font_size, encoding='utf-8')
        tfont = tribesfont.TTFFont()
        for i in range(256):
            fill_color = self.char_colors[i] if not self.fill_trans else "#00000000"
            stroke_color = self.stroke_color if not self.stroke_trans else "#00000000"
            tfont.load_font(font, fill_color, i, self.stroke_width, stroke_color)
        return tfont

    def get_offsets(self):
        res = []
        for i in range(256):
            res.append([self.char_offsets[i][0] + self.offset[0],
                       self.char_offsets[i][1] + self.offset[1]])

        return res

    def get_font_name(self):
        if self.font_location is None:
            return ""
        file_name = os.path.basename(self.font_location)
        return file_name

class Application(tk.Frame):
    selected_gui_layer = None
    selected_char = None
    layer_information = []
    char_width_height = [(0,0)]*256

    def __init__(self, master=None):
        tk.Frame.__init__(self, master)
        self.grid()
        self.createWidgets()
        self.character_set_label = None

    def createWidgets(self):
        left_frame = tk.Frame(self)
        left_frame.grid(row=0, column=0)
        middle_frame = tk.Frame(self)
        middle_frame.grid(row=0, column=1)
        self.right_frame = tk.Frame(self)
        self.right_frame.grid(row=0, column=2)

        # Left Frame
        row = 0
        self.import_button = tk.Button(left_frame, text='Import Layer', command=self.add_layer)
        self.import_button.grid(row=row, columnspan=2)
        row += 1

        self.layer_list = tk.Listbox(left_frame)
        self.layer_list.bind("<<ListboxSelect>>", self.layer_list_select)
        self.layer_list.grid(row=row, columnspan=2)
        row += 1

        self.move_up_layer_button = tk.Button(left_frame, text='Move Up Layer', command=self.move_up)
        self.move_up_layer_button.grid(row=row, columnspan=2)
        row += 1
        self.move_up_layer_button = tk.Button(left_frame, text='Move Down Layer', command=self.move_down)
        self.move_up_layer_button.grid(row=row, columnspan=2)
        row += 1

        self.delete_layer_button = tk.Button(left_frame, text='Delete Layer', command=self.delete_layer)
        self.delete_layer_button.grid(row=row, columnspan=2)
        row += 1

        # Global Controls
        self.spacing_label = tk.Label(left_frame, text="Spacing: ")
        self.spacing_label.grid(row=row, column=0)

        self.spacing_tb = tk.Text(left_frame, width=4, height=1)
        self.spacing_tb.insert(tk.END, "1")
        self.spacing_tb.grid(row=row, column=1)
        row += 1

        self.spacing_label = tk.Label(left_frame, text="Space Char Width (0 = font size): ")
        self.spacing_label.grid(row=row, column=0)

        self.char_spacing_tb = tk.Text(left_frame, width=4, height=1)
        self.char_spacing_tb.insert(tk.END, "0")
        self.char_spacing_tb.grid(row=row, column=1)
        row += 1

        self.baseline_label = tk.Label(left_frame, text="Global baseline offset: ")
        self.baseline_label.grid(row=row, column=0)

        self.baseline_tb = tk.Text(left_frame, width=4, height=1)
        self.baseline_tb.insert(tk.END, "0")
        self.baseline_tb.grid(row=row, column=1)
        row += 1

        self.preview_button = tk.Button(left_frame, text='Save Font', command=self.save_font)
        self.preview_button.grid(row=row, columnspan=2)
        row += 1

        # Middle Frame
        row = 0
        self.font_specific_label = tk.Label(middle_frame, text="Layer Details")
        self.font_specific_label.grid(row=row, column=0)
        row += 1

        self.font_button = tk.Button(middle_frame, text='font', command=self.reselect_font)
        self.font_button.grid(row=row, columnspan=2)
        row += 1

        self.layer_offset = tk.Label(middle_frame, text="Layer Offset X: ")
        self.layer_offset.grid(row=row, column=0)

        self.layer_offsetx_tb = tk.Text(middle_frame, width=4, height=1)
        self.layer_offsetx_tb.insert(tk.END, "0")
        self.layer_offsetx_tb.grid(row=row, column=1)
        row += 1

        self.layer_offsety = tk.Label(middle_frame, text="Layer Offset Y: ")
        self.layer_offsety.grid(row=row, column=0)

        self.layer_offsety_tb = tk.Text(middle_frame, width=4, height=1)
        self.layer_offsety_tb.insert(tk.END, "0")
        self.layer_offsety_tb.grid(row=row, column=1)
        row += 1

        self.layer_save_button = tk.Button(middle_frame, text='Apply Offset', command=self.apply_offset)
        self.layer_save_button.grid(row=row, columnspan=3)
        row += 1

        self.fslbl = tk.Label(middle_frame, text="Font Size: ")
        self.fslbl.grid(row=row, column=0)
        self.font_size_tb = tk.Text(middle_frame, width=4, height=1)
        self.font_size_tb.insert(tk.END, "8")
        self.font_size_tb.grid(row=row, column=1)
        self.apply_button = tk.Button(middle_frame, text='Apply', command=self.apply_size)
        self.apply_button.grid(row=row, column=2)
        row += 1

        self.gc = tk.Label(middle_frame, text="Font Color: ")
        self.gc.grid(row=row, column=0)
        self.global_color_button = tk.Button(middle_frame, text='CLR', background="Black", command=self.select_global_color)
        self.global_color_button.grid(row=row, column=1, columnspan=1)
        self.apply_button = tk.Button(middle_frame, text='Apply', command=self.apply_global_color)
        self.apply_button.grid(row=row, column=2)
        row += 1

        self.swlbl = tk.Label(middle_frame, text="Stroke Width: ")
        self.swlbl.grid(row=row, column=0)
        self.stroke_width_tb = tk.Text(middle_frame, width=4, height=1)
        self.stroke_width_tb.insert(tk.END, "0")
        self.stroke_width_tb.grid(row=row, column=1)
        self.apply_button = tk.Button(middle_frame, text='Apply', command=self.apply_stroke_width)
        self.apply_button.grid(row=row, column=2)
        row += 1

        self.swclbl = tk.Label(middle_frame, text="Stroke Color: ")
        self.swclbl.grid(row=row, column=0)
        self.stroke_color_button = tk.Button(middle_frame, text='CLR', background="Black", command=self.select_stroke_color)
        self.stroke_color_button.grid(row=row, column=1, columnspan=1)
        self.apply_button = tk.Button(middle_frame, text='Apply', command=self.apply_stroke_color)
        self.apply_button.grid(row=row, column=2)
        row += 1

        self.ftlbl = tk.Label(middle_frame, text="Font Transparency: ")
        self.ftlbl.grid(row=row, column=0)
        self.fill_trans_cb = tk.Checkbutton(middle_frame, command=self.toggle_fill_trans)
        self.fill_trans_cb.grid(row=row, column=1)
        row += 1

        self.stlbl = tk.Label(middle_frame, text="Stroke Transparency: ")
        self.stlbl.grid(row=row, column=0)
        self.stroke_trans_cb = tk.Checkbutton(middle_frame, command=self.toggle_stroke_trans)
        self.stroke_trans_cb.grid(row=row, column=1)
        row += 1

        self.aliasing_combo = ttk.Combobox(middle_frame, values=["Default", "Thinly", "Thickly", "Threshold", "Threshold1.11"])
        self.aliasing_combo.grid(row=row, column=0, columnspan=2)
        self.apply_button = tk.Button(middle_frame, text='Apply', command=self.apply_aliasing)
        self.apply_button.grid(row=row, column=2)
        row += 1

        self.char_spec_label = tk.Label(middle_frame, text="Current Character Settings")
        self.char_spec_label.grid(row=row, column=0)
        row += 1

        self.char_pic_label = tk.Label(middle_frame, text="")
        self.char_pic_label.grid(row=row, column=0)
        row += 1

        self.gc = tk.Label(middle_frame, text="Color: ")
        self.gc.grid(row=row, column=0)
        self.color_button = tk.Button(middle_frame, text='CLR', background="Black", command=self.select_color)
        self.color_button.grid(row=row, column=1, columnspan=1)
        self.apply_button = tk.Button(middle_frame, text='Apply', command=self.apply_color)
        self.apply_button.grid(row=row, column=2)
        row += 1

        self.offset = tk.Label(middle_frame, text="Char Offset X: ")
        self.offset.grid(row=row, column=0)

        self.offsetx_tb = tk.Text(middle_frame, width=4, height=1)
        self.offsetx_tb.insert(tk.END, "0")
        self.offsetx_tb.grid(row=row, column=1)
        row += 1

        self.offsety = tk.Label(middle_frame, text="Char Offset Y: ")
        self.offsety.grid(row=row, column=0)

        self.offsety_tb = tk.Text(middle_frame, width=4, height=1)
        self.offsety_tb.insert(tk.END, "0")
        self.offsety_tb.grid(row=row, column=1)
        row += 1

        self.layer_save_button = tk.Button(middle_frame, text='Apply Offset', command=self.apply_char_offset)
        self.layer_save_button.grid(row=row, columnspan=3)
        row += 1

        #Width height adjusts
        self.offset = tk.Label(middle_frame, text="Char Adjustment (Positive # = thinner)")
        self.offset.grid(row=row, columnspan=3)
        row += 1

        self.offset = tk.Label(middle_frame, text="Width Adjust: ")
        self.offset.grid(row=row, column=0)

        self.width_offsetx_tb = tk.Text(middle_frame, width=4, height=1)
        self.width_offsetx_tb.insert(tk.END, "0")
        self.width_offsetx_tb.grid(row=row, column=1)
        row += 1

        self.width_offsety = tk.Label(middle_frame, text="Height Adjust: ")
        self.width_offsety.grid(row=row, column=0)

        self.width_offsety_tb = tk.Text(middle_frame, width=4, height=1)
        self.width_offsety_tb.insert(tk.END, "0")
        self.width_offsety_tb.grid(row=row, column=1)
        row += 1

        self.layer_save_button = tk.Button(middle_frame, text='Apply Adjustment', command=self.apply_char_width_height)
        self.layer_save_button.grid(row=row, columnspan=3)
        row += 1

        self.bg = tk.Label(middle_frame, text="(Application Color)")
        self.bg.grid(row=row)
        self.bg_clr = tk.Button(middle_frame, text='CLR', background="Grey", command=self.select_bg_color)
        self.bg_clr.grid(row=row, column=1, columnspan=1)
        row += 1

    def move_up(self):
        self.move_layer(-1)

    def move_down(self):
        self.move_layer()

    def move_layer(self, direction=1):
        if self.selected_gui_layer is None:
            return

        curr_layer = self.selected_gui_layer
        swap_layer = self.selected_gui_layer + direction

        if swap_layer >= self.layer_list.size() or swap_layer < 0:
            return

        curr_id = self.get_layer_index_from_gui_id(curr_layer)
        swap_id = self.get_layer_index_from_gui_id(swap_layer)

        self.layer_list.delete(curr_layer)
        self.layer_list.insert(curr_layer, f"Layer {swap_id}")

        self.layer_list.delete(swap_layer)
        self.layer_list.insert(swap_layer, f"Layer {curr_id}")

        self.set_list_item(swap_layer)
        self.update_font_layer()

    def set_list_item(self, layer):
        self.layer_list.selection_set(layer)
        self.layer_list_select(None)

    def layer_list_select(self, evt):
        selection = self.layer_list.curselection()
        if selection == ():
            return
        self.selected_gui_layer = selection[0]
        self.load_layer_gui(selection[0])
        self.update_font_layer()

    def load_layer_gui(self, layer_id):
        if layer_id is None:
            return

        true_layer_id = self.get_layer_index_from_gui_id(layer_id)
        gui_info:GuiLayerInformation = self.layer_information[true_layer_id]

        self.font_specific_label.config(text=f"Layer Details: Layer {layer_id}")

        self.font_button.config(text=gui_info.get_font_name())

        self.layer_offsetx_tb.delete(1.0, tk.END)
        self.layer_offsetx_tb.insert(tk.END, str(gui_info.offset[0]))

        self.layer_offsety_tb.delete(1.0, tk.END)
        self.layer_offsety_tb.insert(tk.END, str(gui_info.offset[1]))

        self.font_size_tb.delete(1.0, tk.END)
        self.font_size_tb.insert(tk.END, str(gui_info.font_size))

        self.stroke_width_tb.delete(1.0, tk.END)
        self.stroke_width_tb.insert(tk.END, str(gui_info.stroke_width))

        self.stroke_color_button.configure(bg=gui_info.stroke_color)

        if gui_info.fill_trans:
            self.fill_trans_cb.select()
        else:
            self.fill_trans_cb.deselect()

        if gui_info.stroke_trans:
            self.stroke_trans_cb.select()
        else:
            self.stroke_trans_cb.deselect()

        self.global_color_button.configure(bg=gui_info.char_colors[0])

        self.aliasing_combo.set(gui_info.aliasing)

    def delete_layer(self):
        if self.selected_gui_layer is None:
            return

        layer_id = self.get_layer_index_from_gui_id(self.selected_gui_layer)
        self.layer_list.delete(self.selected_gui_layer)
        self.layer_information[layer_id] = None
        self.update_font_layer()


    def get_layer_index_from_gui_id(self, layer_index):
        layer_index = self.layer_list.get(layer_index).split(' ')[1]
        return int(layer_index)

    def get_layer_index_from_gui_name(self, layer_name):
        layer_index = layer_name.split(' ')[1]
        return int(layer_index)

    def select_stroke_color(self):
        curr_color = self.stroke_color_button.cget("bg")
        rgb, clr = cc.askcolor(color=curr_color)

        self.stroke_color_button.configure(bg=clr)

    def select_global_color(self):
        curr_color = self.global_color_button.cget("bg")
        rgb, clr = cc.askcolor(color=curr_color)

        self.global_color_button.configure(bg=clr)

    def toggle_fill_trans(self):
        if self.selected_gui_layer is None:
            return

        layer_id = self.get_layer_index_from_gui_id(self.selected_gui_layer)
        layer_info:GuiLayerInformation = self.layer_information[layer_id]
        layer_info.fill_trans = not layer_info.fill_trans
        self.update_font_layer()

    def toggle_stroke_trans(self):
        if self.selected_gui_layer is None:
            return

        layer_id = self.get_layer_index_from_gui_id(self.selected_gui_layer)
        layer_info:GuiLayerInformation = self.layer_information[layer_id]
        layer_info.stroke_trans = not layer_info.stroke_trans
        self.update_font_layer()

    def select_color(self):
        if self.selected_char is None:
            messagebox.showinfo("Warning", "Click on a character to edit")
            return
        curr_color = self.color_button.cget("bg")
        rgb, clr = cc.askcolor(color=curr_color)

        self.color_button.configure(bg=clr)

    def select_bg_color(self):
        curr_color = self.bg_clr.cget("bg")
        rgb, clr = cc.askcolor(color=curr_color)

        self.character_set_label.configure(bg=clr)
        self.bg_clr.configure(bg=clr)

    def apply_stroke_color(self):
        if self.selected_gui_layer is None:
            return

        layer_id = self.get_layer_index_from_gui_id(self.selected_gui_layer)
        layer_info:GuiLayerInformation = self.layer_information[layer_id]

        selected_color = self.stroke_color_button.cget("bg")
        layer_info.stroke_color = selected_color
        self.update_font_layer()

    def apply_global_color(self):
        if self.selected_gui_layer is None:
            return

        layer_id = self.get_layer_index_from_gui_id(self.selected_gui_layer)
        layer_info:GuiLayerInformation = self.layer_information[layer_id]

        selected_color = self.global_color_button.cget("bg")
        answer = askyesno("Confirm", "This will override any custom colors for this layers.  Do you wish to proceed?")
        if not answer:
            return
        layer_info.char_colors = [selected_color] * 256
        self.update_font_layer()

    def apply_color(self):
        if self.selected_gui_layer is None:
            return
        if self.selected_char is None:
            return

        layer_id = self.get_layer_index_from_gui_id(self.selected_gui_layer)
        layer_info:GuiLayerInformation = self.layer_information[layer_id]

        selected_color = self.color_button.cget("bg")
        layer_info.char_colors[self.selected_char] = selected_color
        self.update_font_layer()

    def apply_offset(self):
        if self.selected_gui_layer is None:
            return

        layer_id = self.get_layer_index_from_gui_id(self.selected_gui_layer)
        layer_info:GuiLayerInformation = self.layer_information[layer_id]

        offset_x = int(self.layer_offsetx_tb.get(1.0, tk.END))
        offset_y = int(self.layer_offsety_tb.get(1.0, tk.END))
        layer_info.offset = (offset_x, offset_y)
        self.update_font_layer()

    def apply_char_offset(self):
        if self.selected_char is None:
            messagebox.showinfo("Warning", "Click on a character to edit")
            return
        if self.selected_gui_layer is None or self.selected_char is None:
            return

        layer_id = self.get_layer_index_from_gui_id(self.selected_gui_layer)
        layer_info:GuiLayerInformation = self.layer_information[layer_id]

        offset_x = int(self.offsetx_tb.get(1.0, tk.END))
        offset_y = int(self.offsety_tb.get(1.0, tk.END))
        layer_info.char_offsets[self.selected_char] = (offset_x, offset_y)
        self.update_font_layer()

    def apply_char_width_height(self):
        if self.selected_char is None:
            messagebox.showinfo("Warning", "Click on a character to edit")
            return
        if self.selected_gui_layer is None or self.selected_char is None:
            return

        layer_id = self.get_layer_index_from_gui_id(self.selected_gui_layer)
        layer_info:GuiLayerInformation = self.layer_information[layer_id]

        offset_x = int(self.width_offsetx_tb.get(1.0, tk.END))
        offset_y = int(self.width_offsety_tb.get(1.0, tk.END))
        self.char_width_height[self.selected_char] = (offset_x, offset_y)
        self.update_font_layer()

    def apply_size(self):
        if self.selected_gui_layer is None:
            return

        layer_id = self.get_layer_index_from_gui_id(self.selected_gui_layer)
        layer_info:GuiLayerInformation = self.layer_information[layer_id]

        try:
            font_size = int(self.font_size_tb.get(1.0, tk.END))
        except ValueError:
            return
        layer_info.font_size = font_size
        self.update_font_layer()

    def apply_stroke_width(self):
        if self.selected_gui_layer is None:
            return

        layer_id = self.get_layer_index_from_gui_id(self.selected_gui_layer)
        layer_info:GuiLayerInformation = self.layer_information[layer_id]

        try:
            stroke_width = int(self.stroke_width_tb.get(1.0, tk.END))
        except ValueError:
            return
        layer_info.stroke_width = stroke_width
        self.update_font_layer()

    def apply_aliasing(self):
        if self.selected_gui_layer is None:
            return

        layer_id = self.get_layer_index_from_gui_id(self.selected_gui_layer)
        layer_info:GuiLayerInformation = self.layer_information[layer_id]

        alias = self.aliasing_combo.get()
        layer_info.aliasing = alias
        self.update_font_layer()
        return

    def reselect_font(self):
        if self.selected_gui_layer is None:
            return

        filename = self.get_font_path_dialog()

        if filename == "":
            return

        layer_id = self.get_layer_index_from_gui_id(self.selected_gui_layer)
        layer_info:GuiLayerInformation = self.layer_information[layer_id]

        layer_info.font_location = filename
        self.font_button.config(text=layer_info.get_font_name())
        self.update_font_layer()

    def get_font_path_dialog(self):
        filetypes = (
            ('TTF files', '*.ttf'),
            ('All files', '*.*')
        )

        return fd.askopenfilename(title='Open a file', filetypes=filetypes)

    def get_save_path_dialog(self):
        filetypes = (
            ('PFT files', '*.pft'),
            ('All files', '*.*')
        )

        return fd.asksaveasfilename(title='Save a file', filetypes=filetypes)

    def save_font(self):
        if output_font is None:
            return

        self.update_font_layer()

        filename = self.get_save_path_dialog()
        if filename == "":
            return

        name = os.path.basename(filename)
        if name.endswith('.pft'):
            name = name[:-4]
        folder = os.path.dirname(filename)
        spacing = int(self.spacing_tb.get(1.0, tk.END))
        baseline_offset = int(self.baseline_tb.get(1.0, tk.END))
        space_size = int(self.char_spacing_tb.get(1.0, tk.END))
        tribesfont.export_from_ttf(folder, name, output_font, spacing, baseline_offset, space_size, self.char_width_height)

    def add_layer(self):
        filename = self.get_font_path_dialog()

        if filename == "":
            return

        count = len(self.layer_information)

        if self.selected_gui_layer is None:
            gui_info = GuiLayerInformation()
        else:
            gui_info = self.layer_information[self.get_layer_index_from_gui_id(self.selected_gui_layer)].copy()

        gui_info.font_location = filename
        self.layer_list.insert(count, f"Layer {count}")
        self.layer_information.append(gui_info)

        if count == 0:
            self.set_list_item(tk.END)

        self.update_font_layer()

    def character_set_clicked(self, evt):
        if output_font is None:
            return

        if self.selected_gui_layer is None:
            return

        x = evt.x
        y = evt.y

        char_width = self.character_set_label.winfo_width() / 16
        char_height = self.character_set_label.winfo_height() / 16

        x_ind = math.floor(x / char_width)
        y_ind = math.floor(y / char_height)

        index_char = x_ind + y_ind * 16

        img = ImageTk.PhotoImage(output_font.char_images[index_char])

        self.selected_char = index_char
        self.char_pic_label.configure(image=img)
        self.char_pic_label.image = img

        self.width_offsetx_tb.delete(1.0, tk.END)
        self.width_offsetx_tb.insert(tk.END, str(self.char_width_height[index_char][0]))

        self.width_offsety_tb.delete(1.0, tk.END)
        self.width_offsety_tb.insert(tk.END, str(self.char_width_height[index_char][1]))

        self.update_font_layer()

        # print(f"Clicked ind: {x} {y} {char_width} {char_height} {index_char}")

    def update_font_layer(self):
        # Mergy Wergy inefficient but who cares
        font_array = []
        offset_array = []
        layer_id_list = []
        alias_array = []
        width_height_array = []
        for font_item in self.layer_list.get(0, tk.END):
            layer_id_list.append(self.get_layer_index_from_gui_name(font_item))

        layer_id_list.reverse()
        for font_item in layer_id_list:
            font:GuiLayerInformation = self.layer_information[font_item]
            font_array.append(font.get_font())
            offset_array.append(font.get_offsets())
            alias_array.append(font.aliasing)

        global output_font
        output_font = tribesfont.blend_ttf_font(font_array, offset_array, alias_array)

        all_picture_labels.clear()
        draw_png = output_font.to_large_image()

        img = ImageTk.PhotoImage(draw_png)
        if self.character_set_label is None:
            self.character_set_label = tk.Label(self.right_frame, image=img)
            self.character_set_label.bind("<Button-1>", func=self.character_set_clicked)
            self.character_set_label.grid(row=0, column=0)
        self.character_set_label.configure(image=img)
        self.character_set_label.image = img
        bg_clr = self.bg_clr.cget("bg")
        self.character_set_label.configure(bg=bg_clr)
        self.char_pic_label.configure(bg=bg_clr)
        all_picture_labels.append(self.character_set_label)

        if self.selected_char is None or self.selected_gui_layer is None:
            return

        img = ImageTk.PhotoImage(output_font.char_images[self.selected_char])
        self.char_pic_label.configure(image=img)
        self.char_pic_label.image = img

        layer_id = self.get_layer_index_from_gui_id(self.selected_gui_layer)
        offset = self.layer_information[layer_id].char_offsets[self.selected_char]
        self.offsetx_tb.delete(1.0, tk.END)
        self.offsetx_tb.insert(tk.END, str(offset[0]))

        self.offsety_tb.delete(1.0, tk.END)
        self.offsety_tb.insert(tk.END, str(offset[1]))

        self.color_button.configure(bg=self.layer_information[layer_id].char_colors[self.selected_char])

        return

app = Application()
app.master.title('Bov Font - I suck at making GUI\'s Incomplete version Release')
app.mainloop()