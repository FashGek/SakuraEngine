import os
import subprocess
import argparse
import sys
from shutil import copyfile

csoutdir = './../Binaries/Services'

def build_as_csproj(root, file):
    if(file.endswith(".csproj")):
        print('dotnet', 'build', os.path.join(root, file), '-o', './out', '-c', args.configure)
        subprocess.call(['dotnet', 'build',
            os.path.join(root, file), '-o', csoutdir, '-c', args.configure
        ])
    return

def build_as_sln(root, file):
    if(file.endswith(".sln")):
        print('dotnet', 'build', os.path.join(root, file), '-o', './out', '-c', args.configure)
        subprocess.call(['dotnet', 'build',
            os.path.join(root, file), '-o', csoutdir, '-c', args.configure
        ])
    return

def run_as_build_script(root, file):
    source_file = os.path.join(root, file)
    os.system("python3 " + source_file)

def build_as_python(root, file):
    if(file.endswith(".build.py")):
        run_as_build_script(root, file)
        return
    if(file.endswith(".py")):
        source_file = os.path.join(root, file)
        destination_file = os.path.join(csoutdir, file)
        copyfile(source_file, destination_file)
    return

if __name__ == '__main__':
    parser = argparse.ArgumentParser(
        description="This is a description of %(prog)s",
        epilog="This is a epilog of %(prog)s",
        prefix_chars="-", fromfile_prefix_chars="@",
        formatter_class=argparse.ArgumentDefaultsHelpFormatter)
    parser.add_argument("-c", "--configure", 
        dest="configure", 
        choices=['debug', 'release'],
        default="release"
    )
    args = parser.parse_args()
    path = './../Source/'
    for root,dirs,files in os.walk(path):
        for file in files:
            #build_as_csproj(root, file)
            build_as_sln(root, file)
            build_as_python(root, file)