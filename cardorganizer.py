#!/bin/python

#TODO
#timeline sorting
#when same name check if file is identical

import os
import re
import shutil
import argparse
import ahocorasick

games = {
    "KK"   : [("chara", ["【KoiKatuChara】", "【KoiKatuCharaS】", "【KoiKatuCharaSP】"]), ("coordinate", ["【KoiKatuClothes】"]), ("scene", ["【KStudio】"])],
    "KKS"  : [("chara", ["【KoiKatuCharaSun】"])],
    "AI"   : [("chara", ["【AIS_Chara】"]), ("coordinate", ["【AIS_Clothes】"]), ("scene", ["【StudioNEOV2】"]), ("housing", ["【AIS_Housing】"])],
    "EC"   : [("chara", ["EroMakeChara"]), ("hscene", ["EroMakeHScene"]), ("map", ["EroMakeMap"]), ("pose", ["EroMakePose"])],
    "HS"   : [("female", ["【HoneySelectCharaFemale】"]), ("male", ["【HoneySelectCharaMale】"]), ("scene", ["【-neo-】"])],
    "PH"   : [("female", ["【PlayHome_Female】"]), ("male", ["【PlayHome_Male】"]), ("scene", ["【PHStudio】"])],
    "SBPR" : [("female", ["【PremiumResortCharaFemale】"]), ("male", ["【PremiumResortCharaMale】"])],
    "HC"   : [("chara", ["【HCChara】"])],
    "AA2"  : [("chara", ["y�G�f�B�b�g�z"]), ("scene", ["\x00SCENE\x00"])],
    "RG"   : [("chara", ["【RG_Chara】"])]#, ("scene", ["【RoomStudio】"])], # scene token not last in RG
}

sexs = {"\x00" : "male", "\x01" : "female" }


def parse_args():
    parser = argparse.ArgumentParser(description="Sort illusion cards automatically.")
    parser.add_argument("target_dir", help="The directory to search for cards.")
    parser.add_argument("output_dir", help="The directory where cards will be output.")
    parser.add_argument("--subdir", action="store_true", help="Seach subfolders for cards as well.")
    parser.add_argument("--testrun", action="store_true", help="Test the program without moving files.")
    parser.add_argument("--userdata", choices=["KK", "KKS"], help=f"Place cards in correct folders inside output_dir that points to UserData.")
    args = parser.parse_args()
    args.target_dir = os.path.normpath(args.target_dir)
    args.output_dir = os.path.normpath(args.output_dir)
    return args


def create_trie():
    trie = ahocorasick.Automaton()
    for game_name, card_info_list in games.items():
        for pattern_path, patterns in card_info_list:
            for pattern in patterns:
                trie.add_word(pattern, (game_name, pattern_path))
    trie.add_word("sex", "sex")
    trie.make_automaton()
    return trie


def get_card_dir(trie, data, userdata):
    png_end_index = data.find("IEND")
    if png_end_index == -1: return ""

    game_name, pattern_path, sex = "", "", ""
    for end_index, value in trie.iter(data, png_end_index):
        if value == "sex":
            if sex == "":
                temp = data[end_index+1]
                if temp in [*sexs]:
                    sex = temp
        else:
            game_name, pattern_path = value
    if "" in {game_name, pattern_path}: return ""

    if userdata != None:
        allowlist = [userdata]
        if userdata == "KKS": allowlist.append("KK")
        if game_name not in allowlist: return ""
        if pattern_path == "chara" and sex != "":
            pattern_path = os.path.join(pattern_path, sexs[sex])
        if pattern_path == "scene":
            pattern_path = os.path.join("studio", pattern_path)
        return os.path.join(pattern_path, "cardorganizer")
    else:
        if pattern_path == "chara" and sex != "":
            pattern_path = sexs[sex]
        return os.path.join(game_name, pattern_path)


def get_unused_path(dirpath, filename):
    path = os.path.join(dirpath, filename)
    if not os.path.exists(path):
        return (False, path)

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

    return (True, path)


def main():
    args = parse_args()
    if args.testrun: print("Test run, no files will be moved")
    trie = create_trie()
    full_output_dir = os.path.join(os.getcwd(), args.output_dir)

    for dirpath, _, filenames in os.walk(args.target_dir):
        if dirpath.startswith(full_output_dir):
            continue

        for filename in filenames:
            if not filename.endswith(".png"):
                continue

            filepath = os.path.join(dirpath, filename)
            with open(filepath, 'r', errors="replace") as file:
                data = file.read()

            relative_dir = get_card_dir(trie, data, args.userdata)
            if relative_dir != "":
                dest_dir = os.path.join(args.output_dir, relative_dir)
                changed, destpath = get_unused_path(dest_dir, filename)
                msg = os.path.join(relative_dir, os.path.basename(destpath)) if changed else relative_dir
                print(f"'{filename}' -> '{msg}'")
                if not args.testrun:
                    os.makedirs(dest_dir, exist_ok=True)
                    shutil.move(filepath, destpath)

        if not args.subdir:
            break


if __name__ == "__main__":
    main()
