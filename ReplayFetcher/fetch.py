from subprocess import Popen, CREATE_NEW_CONSOLE
import requests
import random
import shutil
import sys
import re
import os

TARGET_VERSION = "4.20.0.315"

SELECTED_CHAMPION_PATTERN = '<th>POV</th><td><ahref="[^"]*">[^<]*</a>as<ahref="/champions/([^/]*)/">'
MATCH_ID_PATTERN = '<a href="/replays/match/([0-9]+)/" class="button navy right">Watch Replay</a>'
REPLAY_ID_PATTERN = 'href="/replays/([0-9]+)/download/"'

def get_filename(champion, replay_id):
    dirname = "Replays/%s" % champion

    if not os.path.exists(dirname):
        os.makedirs(dirname)

    return "%s/%s_%s.lrf" % (dirname, champion, replay_id)

def download_replay(champion, replay_id):
    # print "Downloading replay..."
    filename = get_filename(champion, replay_id)
    url = "http://www.leaguereplays.com/replays/2697801/download/"
    response = requests.get(url, stream=True)

    if response.status_code != 200:
        raise Exception("Failed to retrieve replay")

    if os.path.exists(filename):
        os.remove(filename)

    with open(filename, "wb") as f:
        response.raw.decode_content = True
        shutil.copyfileobj(response.raw, f)

def process_match_page(response):
    # print "Processing match..."
    source = re.sub("\s+", "", response.text)

    try:
        champion = re.search(SELECTED_CHAMPION_PATTERN, source).group(1)
    except:
        if "<tr><th>POV</th><td>Spectator</tr>" not in source:
            with open("champion_error_%i.txt" % int(random.random() * 10000000000000000000000), "a") as f:
                f.write(source.encode("utf-8"))
            return
        champion = "Spectator"

    try:
        replay_id = re.search(REPLAY_ID_PATTERN, source).group(1)
    except:
        with open("replay_error_%i.txt" % int(random.random() * 10000000000000000000000), "a") as f:
            f.write(source.encode("utf-8"))
        return

    download_replay(champion, replay_id)

def fetch_match_page(match_id):
    # print "Fetching match..."
    url = "http://www.leaguereplays.com/replays/match/%s/" % match_id
    response = requests.get(url)
    process_match_page(response)

def process_list_page(response):
    match_ids = []
    for hits in re.finditer(MATCH_ID_PATTERN, response.text):
        match_ids.append(hits.group(1))
    for match_id in match_ids:
        Popen([sys.executable, __file__, "match", match_id]) # creationflags=CREATE_NEW_CONSOLE

def fetch_list_page(index):
    index = int(index) # 1625 is a good default

    while True: # not the best of implementations, but has to do for now
        print "Retrieving page at index %d..." % index
        url = "http://www.leaguereplays.com/replays/?champion=&played=&level=&region=EUNE&sort=&page=%d" % index
        response = requests.get(url)
        process_list_page(response)
        index += 1

def commandline():
    args = sys.argv
    if len(args) < 3:
        raise Exception('Usage: "list {start index}" or "match {match id}"')
    if args[1] == "list":
        return fetch_list_page(args[2])
    if args[1] == "match":
        return fetch_match_page(args[2])
    raise Exception("Invalid parameters")


if __name__ == "__main__":
    commandline()
