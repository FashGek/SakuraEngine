from flask import Flask
import tkinter as tk
from tkinter import filedialog
import sys
app = Flask(__name__)

@app.route('/pick', methods=['POST'])
def pick():
    result = filedialog.askopenfilename()
    root.update()
    return result
    
if __name__ == '__main__':
    root = tk.Tk()
    root.withdraw()
    app.run(host=sys.argv[1], port=sys.argv[2], threaded=False)