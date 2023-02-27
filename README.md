# BakeryAreaLightTool
Editor window tool to automate the tedious process of manually placing area lights

Place the 'BakeryAreaLightTool' in your editor folder.
The BakeryAreaLightManager should be placed in a regular scrips folder.

Use of the tool is pretty straight forward.

You can find the tool under "Bakery/Extensions/Arealight Tool"

# Showcase
https://youtu.be/cQpf7xW_Myc 
(older version but still shows what the tool can do)

# Features
- NEW: automatically detects the right direction to scale the mesh in so you don't have to do it manually anymore
- Select multiple meshes and create a area light based on the bounds to easily place the object accordingly
- Quickly move the object forwards or backwards based on its start position to move it inside or outside the window
- Automatically change intensity, cutoff and color for the newly created area mesh. 
- Actually see what color your mesh will be when baking the light. Standard Bakery doesn't support (only if you make multiple materials for each color) I use property blocks so you can use 1 material. 
- Quickly rotate or flip the mesh for the right orientation
- Source code is obviously available so you can change it to your needs
- A light area manager is provided that automatically disables all area lights in the scene. I have made so much builds and afterwards realised I left my area lights on so I had to rebuild just for that. This manager fixes this. It also makes sure the light mesh doesn't cast shadows that block windows.

# Downsides // possible updates in the feature
- It's currently only made for flat surfaces. If you have a box. It will place itself in the middle of the box. In the future I can change this to use the bounds max extents or something
- If your object doesn't have it's rotation properly setup it can have a wrong rotation. You'll have to fix this manually

# Questions
Feel free to contact me here if you have any questions.


