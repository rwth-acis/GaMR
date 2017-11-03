import sys, bpy, bmesh, os

def triangulate(object):
	mesh = object.data
	temp_mesh = bmesh.new()
	temp_mesh.from_mesh(mesh)
	bmesh.ops.triangulate(temp_mesh, faces=temp_mesh.faces[:], quad_method=0, ngon_method=0)
	temp_mesh.to_mesh(mesh)
	temp_mesh.free()

def convert(conversion_path):
	obj_file = os.path.splitext(conversion_path)[0] + ".x3d"
	bpy.ops.object.select_all(action='SELECT')
	bpy.ops.object.delete()

	print("Importing...")

	if conversion_path.endswith('.obj'):
		bpy.ops.import_scene.obj(filepath=conversion_path)
	elif conversion_path.endswith('stl'):
		bpy.ops.import_mesh.stl(filepath=conversion_path)
	elif conversion_path.endswith('ply'):
		bpy.ops.import_mesh.ply(filepath=conversion_path)
	elif conversion_path.endswith('x3d'):
		bpy.ops.import_scene.x3d(filepath=conversion_path)
	elif conversion_path.endswith('fbx'):
		bpy.ops.import_scene.fbx(filepath=conversion_path)
	elif conversion_path.endswith('3ds'):
		bpy.ops.import_scene.autodesk_3ds(filepath=conversion_path)
	else:
		print("The file format is not supported")
		return

	bpy.ops.object.select_all(action='SELECT')

	# triangulate all imported objects
	selected = bpy.context.selected_objects
	print("Triangulating mesh...")
	for imported_object in selected:
		triangulate(imported_object)

	print("Exporting...")
	bpy.ops.export_scene.x3d(filepath=obj_file)

	print("Finished!")

def main():
	argv = sys.argv
	conversion_path = argv[argv.index("--")+1]
	convert(conversion_path)

main()