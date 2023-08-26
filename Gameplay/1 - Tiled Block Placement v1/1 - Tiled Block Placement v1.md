# Gameplay 1 - Tiled Block Placement v1
This prototype divides the world into tiles/cells and we can mark each tile with a custom marker (e.g. a construction tile, residence tile, road tile, etc).
We can place a prefab (some boxes in this case), drag it into any part of the map, rotate it, and if the place is not occupied, we can place the prefab and mark the new occupied space.

## Test
Some instructions to test the prototype:
 - Press 'A' to place "prefab1" and 'B' for "prefab2"
 - Press 'R' to rotate object to be placed.
 - Press 'Space' to place object (if the place is taken, the object is not placed and disappears)
 - Press 'Escape' to delete the object to be placed
 - Click the object to be placed to drag it across the map

## Considerations
 - The area a prefab takes is calculated based on a the smallest box collider that wraps up the prefab.

## TODO
 - Change material/shader of object to visualize if it can be placed in a location or not
 - Select placed object and move/delete it
 - Add intelligent tiles for the roads so they can change its orientation and the type of road when connecting multiple tiles.
 - Add tiles that instead of placing a sprite on the grid, add a prefab.
 - Ability to paint/select a rectangle in the grid (for procedural buildings)
