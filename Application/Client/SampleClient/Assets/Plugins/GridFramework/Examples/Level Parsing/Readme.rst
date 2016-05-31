.. This document is using the reStructuredText markup format
.. default-role:: code

#######################################
Designing a level from an external file
#######################################

If you have  a game that  is made of  regular tiles you  might want to load the
levels from a file.  In this example we use a very simple text file to load the
level  information  and Grid  Framework to  position  the  tiles  in  the world
accordingly.


Files overview
##############

===============  ===================================================
Name             Description
===============  ===================================================
LevelParser.cs   The only script of the example.
LevelFiles/      Text files for the individual levels.
Colors/          Materials for the blocks.
Prefabs/         Prefabs of the blocks.
===============  ===================================================


How it works
############

We are using the .Net class `StringReader`  to read the level file, one line at
a time  and every  line one  character at  a time.  The row  and column of each
character are used as  the *X*- and *Y* coordinates of the corresponding block.
From there on it just requires converting the grid coordinates world-space.

.. code-block:: c#

   var position = grid.HerringUToWorld(new Vector3(column, row, 0));
   var block = Char2Object(line[column], r, g, b);
   CreateBlock(block, position);

We use hexagonal  grids in this example,  but it works  with any  other type of
grid as well,  just replace the herringbone  coordinate system with the one you
want to use instead.

There is one minor caveat:  the text file starts in the upper right-hand corner
while the grid starts in the lower right-hand corner. It would be preferable if
the layout of the characters matched that of the grid. For this reason the grid
has been rotated 180° along its *X*-axis.
