from flask import Flask
from tkinter import filedialog
from platform import system as platform
from os import system
import sys
import tkinter as tk
app = Flask(__name__)

@app.route('/version', methods=['POST'])
def version():
    return "0.1.0"

@app.route('/pick', methods=['POST'])
def pick():
    if platform() == 'Darwin': 
        system('''/usr/bin/osascript -e 'tell app "Finder" to set frontmost of process "Python" to true' ''')
    result = filedialog.askopenfilename(parent=root)
    root.update()
    return result
    
if __name__ == '__main__':
    root = tk.Tk()
    root.withdraw()
    app.run(host=sys.argv[1], port=sys.argv[2], threaded=False)