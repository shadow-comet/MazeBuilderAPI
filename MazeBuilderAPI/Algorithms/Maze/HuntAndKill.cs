﻿namespace MazeBuilderAPI.Algorithms.Maze;

using Models.Enums;
using Models.Internal;

public class HuntAndKill : MazeBuilderBaseAlgorithm
{
    public override MazeAlgorithm MazeAlgorithmName  => MazeAlgorithm.HuntAndKill;
    protected override bool ShouldInitializeWalls => true;

    public override void Generate()
    {
        // Track visited vertices, initializing with all unvisited
        var visited = Enumerable.Repeat(false, Rows * Columns).ToList();

        // Visiting the first vertex
        var currentX = RandomStream.Next(0, Rows-1);
        var currentY = RandomStream.Next(0, Columns-1);
        visited[GetVertexIndex(currentX, currentY)] = true;

        bool bHuntPhase = false;

        while (true)
        {
            // Set vertex neighbors
            var neighbors = new List<IntPoint>();

            // Looking for non visited neighbors
            foreach (var direction in Directions.OrderBy(d => RandomStream.Next()))
            {
                var newX = currentX + direction.X;
                var newY = currentY + direction.Y;

                if (IsValidVertex(newX, newY) && !visited[GetVertexIndex(newX, newY)])
                {
                    neighbors.Add(new IntPoint(newX, newY));
                }
            }

            // Found vertex's non visited neighbor
            if (neighbors.Count > 0)
            {
                // Choose random neighbor
                var chosenNeighbor = neighbors[RandomStream.Next(0, neighbors.Count-1)];

                // Add a vertex between current vertex and the current neighbor
                HandleWallBetween(currentX, currentY, chosenNeighbor.X, chosenNeighbor.Y, true);

                // Move the current vertex to the selected neighbor and check as visited
                currentX = chosenNeighbor.X;
                currentY = chosenNeighbor.Y;
                visited[GetVertexIndex(currentX, currentY)] = true;

                bHuntPhase = false;  // Still on "Kill" phase
            }

            // Start hunt phase if all neighbors are already visited
            else if (!bHuntPhase)
            {
                bHuntPhase = true;
                var bFoundNewStart = false;

                // Look for a non visited vertex that has at least one visited neighbor
                for (var y = 0; y < Columns; ++y)
                {
                    for (var x = 0; x < Rows; ++x)
                    {
                        // Found a non visited vertex
                        if (!visited[GetVertexIndex(x, y)])
                        {
                            // Check if it has a visited neighbor
                            foreach (var direction in Directions)
                            {
                                var newX = x + direction.X;
                                var newY = y + direction.Y;

                                // Connect the yet not visited vertex to the visited neighbor
                                if (IsValidVertex(newX, newY) && visited[GetVertexIndex(newX, newY)])
                                {
                                    HandleWallBetween(x, y, newX, newY, true);
                                    currentX = x;
                                    currentY = y;
                                    visited[GetVertexIndex(currentX, currentY)] = true;
                                    bFoundNewStart = true;
                                    break;
                                }
                            }
                            if (bFoundNewStart)
                            {
                                break;
                            }
                        }
                    }
                    if (bFoundNewStart)
                    {
                        break;
                    }
                }
                if (!bFoundNewStart)
                {
                    break;
                }
            }
            // Finish current hunt
            else
            {
                bHuntPhase = false;
            }
        }
    }
}