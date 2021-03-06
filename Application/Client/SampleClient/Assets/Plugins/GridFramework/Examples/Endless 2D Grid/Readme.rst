.. This document is using the reStructuredText markup format
.. default-role:: code

###############
Endless 2D grid
###############

The larger a grid to render, the more expensive it gets due to all the lines we
need to compute  and render.  To mitigate this we  can fake an infinitely large
grid by adjusting  the range of the  renderer behind  the scenes as  the camera
moves around.


Files overview
##############

=================  ======================================================
Name               Description
=================  ======================================================
InfinityCamera.cs  The script that moves the camera and adjusts the grid.
=================  ======================================================


How it works
############

For this example  we don't even have  to change the  grid itself,  we only care
about its renderer. A `Parallelepiped` renderer use a lower and upper bound. We
initially set the bounds to go slightly beyond what the camera can see and once
the camera gets close enough to the edge we shift both bounds accordingly.

.. code-block:: c#

   private void ResizeGrid () {
      Vector3 shift = Vector3.zero;
      for (var i = 0; i < 2; ++i) {
         var current = transform.position[i];
         var last    = _lastPosition[i];
   
         shift[i] += current - last;
      }
   
      // Add the shift to both values
      _renderer.From += shift;
      _renderer.To   += shift;
   
      _lastPosition = transform.position;
   }
