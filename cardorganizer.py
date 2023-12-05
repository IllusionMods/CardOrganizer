#!/bin/python

#TODO
#if output detected as a game folder move files to userdata in correct folders, cards for unspecified games wont be moved
#skip PNG segment
#overwriting prompt
#dont read cards from output dir
#timeline sorting

import os
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
    parser.add_argument("output_dir", help="The directory where cards will be output")
    parser.add_argument("--subdir", action="store_true", help="Seach subfolders for cards as well.")
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
                sex_temp = int.from_bytes(str.encode(data[end_index+1]))
                if sex_temp in {0, 1}:
                    sex = sex_temp
        else:
            game_name, pattern_path = value

    if pattern_path == "chara":
        if sex == 0: pattern_path = "male"
        elif sex == 1: pattern_path = "female"

    return os.path.join(game_name, pattern_path)

if __name__ == "__main__":
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
                destpath = os.path.join(args.output_dir, relative_dir, filename)
                if os.path.exists(destpath):
                    print(f"'{filename}' exists in destination already")
                else:
                    print(f"'{filename}' -> '{relative_dir}'")
                    if not args.testrun:
                        os.makedirs(os.path.dirname(destpath), exist_ok=True)
                        shutil.move(filepath, destpath)

        if not args.subdir:
            break
