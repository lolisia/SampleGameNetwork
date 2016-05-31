.. This document is using the reStructuredText markup format
.. default-role:: code

#######################
Terrain mesh generation
#######################

We can use the grid to generate and manipulate a mesh at runtime. The initial
shape of the map is loaded from a height map text file that lists the height of
every vertex, and while the game is running the player can raise of lower a
vertex by clicking.

Files overview
##############

======================  =======================================================
Name                    Description
======================  =======================================================
TerrainMeshBuilder.cs   Build and manipulate the mesh and maintain height data.
HeightMap.txt           The initial heights serialized as a text file.
======================  =======================================================


How it works
############

The height map format is as simple as it gets, we use only positive integer
characters and every character is a vertex. When the game loads we first read
the heights into a flat array of integers.

.. code-block:: c#

   private Vector3[] AssignVertices(Mesh mesh) {
      var vertices = new Vector3[rows * columns];
      
      for (var i = 1; i <= rows; i++){
         for (var j = 1; j <= columns; j++) {
            // Pick the height from text. The row and column get
            // translated into the index within the long text string.
            var index = RowColumn2Index(i, j);
            var h = heights[index];
            // Now assign the vertex depending on its row, column and
            // height (vector in local space; row, column and height in
            // grid space)
            var  gridVertex = new Vector3(j - 1, h, -i + 1);
            var worldVertex = _grid.GridToWorld(gridVertex);
            var localVertex = transform.InverseTransformPoint(worldVertex);
            vertices[index] = localVertex;
         }
      }
      
      // Assign the vertices to the mesh
      mesh.vertices = vertices;
      return vertices;
   }

As you can see we take the row and column of a vertex as its grid coordinates
and then convert that to world and finally to local to be usable in a mesh.
This is how you assign vertices to a mesh in Unity.

To raise a vertex we get its index and then add a multiple of the grid's `Up`
to that vertex and re-assign the vertices back to the mesh.

.. code-block:: c#

   private void RaiseVertex(int index, int amount) {
      var vertices = _mesh.vertices;
      var up = amount * _grid.Up;
      
      vertices[index] += up;
      _mesh.vertices = vertices;
      
      heights[index] += amount;
   }

Getting the row and column of a vertex from world-coordinates is fairly easy as
well:

.. code-block:: c#

   private int HandleMouseInput() {
      var input = Input.mousePosition;
      RaycastHit hit;
      _mc.Raycast(Camera.main.ScreenPointToRay(input), out hit, Mathf.Infinity);
      
      var gridPoint = _grid.WorldToGrid(hit.point);
      var i = Mathf.RoundToInt(-gridPoint.z) + 1;
      var j = Mathf.RoundToInt(gridPoint.x) + 1;
      
      return RowColumn2Index(i, j);
   }

We convert the world-coordinates to grid-coordinates and then round the `x`-
and `z` component to use as row and column respectively.

