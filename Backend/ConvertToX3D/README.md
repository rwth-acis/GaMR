The converter script requires that [Blender](https://www.blender.org/) is installed. On Windows you should add the installation folder of Blender to the PATH-variable (or alternatively navigate to the folder with the command window and run the command from there). Make sure that the file `convertToX3D.py` is also situated in the current folder (alternatively you can state the full path to this script).

To convert a file, open the console and call the following command. On Windows you may need to run the console as administrator.

    blender -b --python convertToX3D.py -- pathTo3DFile

Replace `pathTo3DFile` with the path to your 3D file. 

The conversion script creates a X3D-file with the same name as the conversion file in the folder of the original 3D file.