#!/bin/python

#TODO
#if output detected as a game folder move files to userdata in correct folders, cards for unspecified games wont be moved
#skip PNG segment
#dont read cards from output dir
#timeline sorting
#when same name check if file is identical

import os
import re
import shutil
import argparse
import ahocorasick

games = {
    "KK"   : [("chara", "【KoiKatuChara】"), ("chara", "【KoiKatuCharaS】"), ("chara", "【KoiKatuCharaSP】"), ("outfit", "【KoiKatuClothes】"), ("studio", "【KStudio】")],
    "KKS"  : [("chara", "【KoiKatuCharaSun】")],
    "AI"   : [("chara", "【AIS_Chara】"), ("outfit", "【AIS_Clothes】"), ("studio", "【StudioNEOV2】"), ("housing", "【AIS_Housing】")],
    "EC"   : [("chara", "EroMakeChara"), ("hscene", "EroMakeHScene"), ("map", "EroMakeMap"), ("pose", "EroMakePose")],
    "HS"   : [("female", "【HoneySelectCharaFemale】"), ("male", "【HoneySelectCharaMale】"), ("studio", "【-neo-】")],
    "PH"   : [("female", "【PlayHome_Female】"), ("male", "【PlayHome_Male】"), ("studio", "【PHStudio】")],
    "SBPR" : [("female", "【PremiumResortCharaFemale】"), ("male", "【PremiumResortCharaMale】")],
    "HC"   : [("chara", "【HCChara】")],
    "AA2"  : [("chara", "y�G�f�B�b�g�z"), ("studio", "\x00SCENE\x00")],
    "RG"   : [("chara", "【RG_Chara】")]#, ("studio", "【RoomStudio】")], # studio token not last in RG
}

def parse_args():
    parser = argparse.ArgumentParser(description="Sort illusion cards automatically.")
    parser.add_argument("target_dir", help="The directory to search for cards.")
    parser.add_argument("output_dir", help="The directory where cards will be output.")
    parser.add_argument("--subdir", action="store_true", help="Seach subdirectories for cards as well.")
    parser.add_argument("--testrun", action="store_true", help="Test the program without moving files.")
    return parser.parse_args()

def create_trie():
    trie = ahocorasick.Automaton()
    for game_name, card_info_list in games.items():
        for pattern_path, pattern in card_info_list:
            trie.add_word(pattern, (game_name, pattern_path))
    trie.add_word("sex", "sex")
    trie.make_automaton()
    return trie

def get_card_dir(trie, data):
    game_name, pattern_path, sex = "", "", -1
    for end_index, value in trie.iter(data):
        if value == "sex":
            if sex == -1:
                sex_temp = int.from_bytes(str.encode(data[end_index+1])) #use char directly
                if sex_temp in {0, 1}:
                    sex = sex_temp
        else:
            game_name, pattern_path = value

    if pattern_path == "chara":
        if sex == 0: pattern_path = "male"
        elif sex == 1: pattern_path = "female"

    return os.path.join(game_name, pattern_path)

def get_unused_path(dirpath, filename):
    path = os.path.join(dirpath, filename)
    if not os.path.exists(path):
        return path

    index = 1
    base, ext = os.path.splitext(filename)
    match = re.match("^(.+)\(([0-9])\)$", base)
    if match != None:
        new_base, new_index = match.groups()
        filename = f"{new_base.strip()}{ext}"
        index = int(new_index)

    while True:
        base, ext = os.path.splitext(filename)
        path = os.path.join(dirpath, f"{base} ({index}){ext}")
        index += 1
        if not os.path.exists(path): break

    return path

def main():
    args = parse_args()
    trie = create_trie()

    if args.testrun:
        print("Test run, no files will be moved")
    
    for dirpath, _, filenames in os.walk(args.target_dir):
        for filename in filenames:
            if not filename.endswith(".png"):
                continue

            filepath = os.path.join(dirpath, filename)
            with open(filepath, 'r', errors="replace") as file:
                data = file.read()

            relative_dir = get_card_dir(trie, data)
            if relative_dir != "":
                dest_dir = os.path.join(args.output_dir, relative_dir)
                destpath = get_unused_path(dest_dir, filename)
                print(f"'{filename}' -> '{os.path.join(relative_dir, os.path.basename(destpath))}'")
                if not args.testrun:
                    os.makedirs(dest_dir, exist_ok=True)
                    shutil.move(filepath, destpath)

        if not args.subdir:
            break

if __name__ == "__main__":
    main()
