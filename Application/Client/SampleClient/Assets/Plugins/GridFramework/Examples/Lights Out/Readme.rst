.. This document is using the reStructuredText markup format
.. default-role:: code

################################
Lights-out game with polar grids
################################

This is a puzzle game where the goal is to  turn off all the lights by clicking
them.  Every time the player clicks a  light that light and all adjacent lights
toggle their state. In this example the lights have been randomly set and it is
not guaranteed that the game can actually be solved.

This example demonstrates several concepts: at the top-level we are writing out
game logic entirely using  grid-coordinates and we don't concern ourselves with
how it all maps to world-space.

We also  use a  custom extension  method to  keep the game logic clean from any
implementation details.  The extension  method contains  all the  logic that is
required to decide when two tiles are adjacent in the grid.

Finally,  all the tiles are generated at runtime:  we use the grid's properties
to compute the  vertices for the meshes.  That way it doesn't matter how we set
up our grid,  building the  game is automated.  This is  very useful  for level
designers who  can then  write the  individual puzzles  in a  text-based  level
format and have the file parsed. See the *Level Parsing* example for details.


Files overview
##############

=======================   ===================================================
Name                      Description
=======================   ===================================================
LightsBehaviour.cs        The individual behaviour of every script
LightsManager.cs          Coordinates handling of click events.
ConstructPolarTiles.cs    Sets up the puzzle by building the tiles.
Extensions.cs             Extension method for polar grids.
Materials/                Materials for the lights.
Prefabs/                  Prefabs of the tiles.
=======================   ===================================================


How it works
############

As mentioned  above  the example implements  three different  concepts.  At the
topmost level when  the player clicks  a tile an  event is fired.  Every single
tile is subscribed to that event and reacts accordingly.


Game logic
==========

On start  every grid  subscribes to  the click event and  when a light has been
clicked the  lights manager will send  and event.  The clicked light passes its
own position to the manager,  the manager passes it on to every receiving light
and every light compares them to its own position.  If the light is adjacent it
toggles its state, and adjacency is determined by an extension method of the
grid.

.. code-block:: c#

   var isAdjacent = grid.IsAdjacent(transform.position, reference);
   
   if (isAdjacent) {
      _isOn = !_isOn;
      SwitchLights();
   }


Determining adjacency
=====================

The rules for adjacency are very simple:  if the difference between coordinates
is :math:`-1`, :math:`0` or :math:`1` they are adjacent.  If we want to exclude
diagonals we require that the sum of the absolute difference is no more than
:math:`1`.

.. code-block:: c#

   a = grid.WorldToGrid(a);
   b = grid.WorldToGrid(b);
   
   bool adjacent = Delta(a.x, b.x) + Delta(a.y, b.y) <= 1.25;
   
   float Delta(float a, float b) {
   	return Mathf.Abs(a - b);
   }

Since this example uses polar grid  for an extra challenge we also have to take
wrapping around in consideration:

.. code-block:: c#

   var sectors = grid.Sectors;
   if (Delta(a.y, sectors) <= .75f && b.y <= .75f) {
   	b.y += sectors;
   }
   if (Delta(b.y, sectors) <= .75f && a.y <= .75f) {
   	a.y += sectors;
   }


Building tiles
==============

Tiles are build for every ring around the origin and every sector per ring. The
tiles  are made of  pairs of vertices,  one inner (towards the origin)  and one
outer (away from the origin),  the amount of pairs depends on the smoothness of
the renderer.

.. code-block:: c#

   var smoothness = _cylinder.Smoothness;
   var subSectors = smoothness + 1;
   
   for (var k = 0; k < subSectors; ++k) {
   	var subSector = sector + (float)k / smoothness;
   
   	var innerVertex = new Vector3(ring, subSector, 0);
   	var outerVertex = innerVertex + Vector3.right;
   
   	innerVertex = _grid.GridToWorld(innerVertex);
   	outerVertex = _grid.GridToWorld(outerVertex);
   }

From there on we assign the vertices just like for any other mesh in Unity.
