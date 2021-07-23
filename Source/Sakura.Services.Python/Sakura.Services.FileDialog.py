from flask import Flask
import tkinter as tk
from tkinter import filedialog
import sys
app = Flask(__name__)

@app.route('/pick', methods=['POST'])
def pick():
    return filedialog.askopenfilename()
    
if __name__ == '__main__':
    tk.Tk().withdraw()
    app.run(host=sys.argv[1], port=sys.argv[2], threaded=False)