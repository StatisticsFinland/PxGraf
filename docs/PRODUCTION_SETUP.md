# Production setup

Because technical details or requirements of the target anvironment will vary, here are some high-level examples to give an idea how the PxGraf can be set up.

Statistics Finland will **not** offer any support for the production setup of PxGraf. The following examples are provided as is and are not guaranteed to work in all environments.

Some good things to know:
	- The backend is stateless, it can be scaled horizontally as long as the instances can access the query files.
	- The backend does not support authentication, if you want to limit access to the creation API for example that needs to be handled outside of PxGraf.
	- The query files are read-only once they are created. PxGraf never edits or deletes them.
	- The frontend SPA is a static website, it can be served by any web server that can serve static files. (The backend serves it by default.)

## Only one PxGraf instance

This is the simplest setup. A single instance if PxGraf that serves the frontend SPA and runs the visualization API.

![PxGraf setup with one instance](/pxgraf_setup_1.png)

## Two PxGraf instances

In some cases it might be beneficial to have separate instances for the creation api and the visualization api. This enables authentication on instance basis and independent scaling for example. All of the instances must access the same query files and the data received from the PxWeb instances must be identical if there are more than one.

![PxGraf setup with two instances](/pxgraf_setup_2.png)