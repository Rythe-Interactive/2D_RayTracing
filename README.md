# Guided Learning Minor CMGT
During my minor I will be trying to make a 2D raytracer. 

# Relevance
The industry is working towards improving real-time raytracing in 3D. 3D real-time ray tracing has of course been applied on a vew games. Before applying it on games, Ray tracing was already used in offline rendering for, for example, movies.
I have not yet found a 2D ray tracer with the same application. Raycasting has been done in 2D for player view, or to find the area which a light would light up, but it has not been applied for actual dynamic lighting and/or global illumination. Because more games are being equipped with a ray traced renderer, it becomes more relevent to work with it in any way, and to find optimizations.

# Implementation
As mentioned this repository is to keep track of a 2D ray tracer protoype. There are two implementations. A very early prototype made in processing, and a unity prototype. The unity prototype is the most recent. Up until now the unity tracer has ray-circle collision and a collider for rectangular backgrounds, which only receive light when the light is on the background. Moreover, the ray tracer will only be updated when something in the scene makes a light change. 
## Rendering
Both ray tracers, in unity and processing, are hybrid renderers, in that the light information is send to a rastiraztion shader which will compute the light information. Currently the rays are traced on CPU, which has a huge cost on performance.
