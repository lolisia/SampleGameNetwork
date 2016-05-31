.. This document is using the reStructuredText markup format
.. default-role:: code

##################################
Grid-based movement with obstacles
##################################

This example demonstrates two things: moving from tile to tile in a grid-based
fashion and using a grid to map between the game world and a tile-based model
world. Every step the player picks a direction and then checks whether the
target tile is occupied or not.

The example employs a weak form of the model-view-controller pattern, but the
distinction is not a strict as in a real MVC implementation.


Files overview
##############

===============    =========================================================
Name               Description
===============    =========================================================
ForbiddenFiles.cs  Model- and controller script for the game.
RoamGrid.cs        Attached to the player to make it move around the world.
ModelGrid.cs       Attached to the grid so it can register itself to the
                   controller.
BlockSquare.cs     Attached to each obstacle so can register itself to the
                   model.
===============    =========================================================


How it works
############

The game is managed by a static class that maintains references to a grid and
its renderer. The game is modelled by a two-dimensional array of `bool` values,
where the index into the array corresponds to what the playing field looks
like. The size of the array is determined by the range of the renderer.

.. code-block:: c#

   private static int[] SetMatrixSize() {
      var size = new int[2];
      for(var i = 0; i < 2; ++i){
         var from = Mathf.CeilToInt( _renderer.From[i]);
         var to   = Mathf.FloorToInt(_renderer.To[i]  );
   
         size[i] = to - from;
      }
      return size;
   }

After the matrix has been built we need to mark the occupied tiles. This is
done by the obstacles individually: each one sends its coordinates to the
manager, the manager computes the index into the array and sets the value to
`false`.

.. code-block:: c#

   private static int[] GetSquare(Vector3 vec){
      const CoordinateSystem system = CoordinateSystem.Grid;
      
      var cell = _grid.NearestCell(vec, system);
      
      var square = new [] {
         Mathf.FloorToInt(cell.x) - _originSquare[0],
         Mathf.FloorToInt(cell.y) - _originSquare[1],
      };
      
      return square;
   }

Note that we are using an official extension method here to find the nearest
cell.

Moving the player works by first picking a direction randomly and then checking
the destination in the model. When we have the destination we use the above
method to find the index, check the value of the matrix and then either accept
or reject the target.
