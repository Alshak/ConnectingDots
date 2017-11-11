/**
Connecting Squares, a Unity Open Source Puzzle game
Copyright (C) 2017  Alain Shakour

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
**/
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{

    public GameObject playerTemplate;
    public GameObject nextPlayerTemplate;
    public GameObject columnCellTemplate;
    public GameObject floorTemplate;

    public GameObject leftColumn;
    public GameObject rightColumn;
    public GameObject mainCamera;
    public GameObject debugGlobalController;

    private const int leftColumnIndex = 0;
    private const int rightColumnIndex = 6;
    private const int columnSize = 15;
    private const float blocksAutofallSpeed = 0.4f;
    private const float columnRecreationSpeed = 0.2f;
    private IEnumerator coroutine;
    private GameObject nextPlayer;
    private GlobalController globalController;


    void Start()
    {
        var menuGlobalController = GameObject.FindGameObjectWithTag(TagNames.GlobalController);
        if (menuGlobalController == null)
        {
            debugGlobalController.SetActive(true);
            globalController = debugGlobalController.GetComponent<GlobalController>();
            globalController.GameSpeed = 5;
        }
        else
        {
            globalController = menuGlobalController.GetComponent<GlobalController>();
        }
        CreateColumns();
        CreateNextPlayer();
        CreatePlayer();
        CreateFloor();
        SetupCamera();
    }

    void Update()
    {
        bool escape = Input.GetButtonDown("Escape");
        if (escape)
        {
            EndGame();
        }
    }

    private void SetupCamera()
    {
        mainCamera.transform.position = new Vector3((rightColumnIndex + leftColumnIndex) / 2, columnSize * 0.6f, -10);
        mainCamera.GetComponent<Camera>().orthographicSize = (rightColumnIndex - leftColumnIndex) * columnSize * 0.12f;
    }


    public void DoNewCycle()
    {
        coroutine = StepByStepBlockMovement(blocksAutofallSpeed / globalController.GameSpeed);
        StartCoroutine(coroutine);

    }

    private IEnumerator StepByStepBlockMovement(float waitTime)
    {
        List<Transform> flyingCell = new List<Transform>();
        do
        {
            List<Transform> allCells = ComputeConnectingSquares();
            flyingCell = ComputeFlyingCellNewPosition(allCells);
            if (flyingCell.Count != 0)
            {
                yield return new WaitForSeconds(waitTime);
            }
            while (MoveCellToLowerPosition(flyingCell))
            {
                yield return new WaitForSeconds(waitTime);
            }
        } while (flyingCell.Count != 0);
        yield return RecreateColumns();
        if (!IsEndGame())
        {
            CreatePlayer();
        }
        else
        {
            EndGame();
        }
    }

    #region Block color

    public List<int> GetNextPlayerColors()
    {
        BlockColor[] blockColors = nextPlayer.GetComponentsInChildren<BlockColor>();
        List<int> colors = blockColors.Select(c => c.Color).ToList();
        return colors;
    }
    #endregion

    #region Create every objects
    private void CreateColumns()
    {
        bool leftColumnCreation = CreateColumn(leftColumn, leftColumnIndex, columnSize, TagNames.LeftColumn);
        bool rightColumnCreation = CreateColumn(rightColumn, rightColumnIndex, columnSize, TagNames.RightColumn);
        while (leftColumnCreation || rightColumnCreation)
        {
            leftColumnCreation = CreateColumn(leftColumn, leftColumnIndex, columnSize, TagNames.LeftColumn);
            rightColumnCreation = CreateColumn(rightColumn, rightColumnIndex, columnSize, TagNames.RightColumn);
        }
    }

    private IEnumerator RecreateColumns()
    {
        bool leftColumnCreation = CreateColumn(leftColumn, leftColumnIndex, columnSize, TagNames.LeftColumn);
        bool rightColumnCreation = CreateColumn(rightColumn, rightColumnIndex, columnSize, TagNames.RightColumn);
        while (leftColumnCreation || rightColumnCreation)
        {
            leftColumnCreation = CreateColumn(leftColumn, leftColumnIndex, columnSize, TagNames.LeftColumn);
            rightColumnCreation = CreateColumn(rightColumn, rightColumnIndex, columnSize, TagNames.RightColumn);
            yield return new WaitForSeconds(columnRecreationSpeed);
        }
    }

    private bool CreateColumn(GameObject column, int worldPosition, int nbCellsPerColumn, string tagName)
    {
        BlockColor[] cells = column.GetComponentsInChildren<BlockColor>();
        if (cells.Count() < nbCellsPerColumn)
        {
            GameObject newCell = Instantiate(columnCellTemplate, new Vector2(worldPosition, cells.Count() + 1f), Quaternion.identity);
            newCell.transform.parent = column.transform;
            newCell.tag = tagName;
            newCell.GetComponent<BlockController>().ChangeNameForDebug();
            return true;
        }
        return false;
    }

    private void CreateFloor()
    {
        float padding = leftColumnIndex;
        while (padding <= rightColumnIndex)
        {
            GameObject floor = Instantiate(floorTemplate, new Vector2(padding, 0), Quaternion.identity);
            floor.GetComponent<BlockColor>().Color = 6;
            padding++;
        }
        float ceilingSize = 0.5f;
        padding = leftColumnIndex + ceilingSize * 1.5f;
        while (padding <= rightColumnIndex - ceilingSize)
        {
            GameObject ceiling = Instantiate(floorTemplate, new Vector2(padding, columnSize + ceilingSize * .5f), Quaternion.identity);
            ceiling.transform.localScale = new Vector2(ceilingSize, ceilingSize);
            ceiling.GetComponent<BlockColor>().Color = 7;
            ceiling.GetComponent<SpriteRenderer>().sortingLayerName = SortingLayerNames.Background;
            ceiling.GetComponent<BoxCollider2D>().enabled = false;
            padding += ceilingSize;
        }
    }

    private bool IsEndGame()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag(TagNames.Player);

        // Check the maximum Y position of all the blocks in the current game
        float maxPlayerPositionInY = players.Length > 0 ? players.Max(p => p.transform.position.y) : 0;

        return maxPlayerPositionInY >= columnSize;
    }

    private void CreatePlayer()
    {
        Instantiate(
                playerTemplate,
                new Vector2((int)System.Math.Round((leftColumnIndex + rightColumnIndex) / 2d, System.MidpointRounding.AwayFromZero), columnSize + .5f),
                playerTemplate.transform.rotation);
    }

    private void CreateNextPlayer()
    {
        nextPlayer = Instantiate(
            nextPlayerTemplate,
            new Vector2(rightColumnIndex + 3, columnSize - 3),
            nextPlayerTemplate.transform.rotation);
    }

    public void ChangeNextPlayerColors()
    {
        BlockColor[] blockColors = nextPlayer.GetComponentsInChildren<BlockColor>();
        foreach (BlockColor blockColor in blockColors)
        {
            blockColor.Color = globalController.GetRandomColor();
        }
    }
    #endregion

    #region Compute Connecting Squares
    private List<Transform> ComputeConnectingSquares()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag(TagNames.Player);

        Transform[] leftCells = leftColumn.GetComponentsInChildren<Transform>();
        Transform[] rightCells = rightColumn.GetComponentsInChildren<Transform>();
        Transform[] playersTransform = players.SelectMany(p => p.GetComponentsInChildren<Transform>()).ToArray();

        List<Transform> allCells = leftCells.Concat(playersTransform).Concat(rightCells).Where(p => p.GetComponent<BlockController>() != null).OrderBy(p => p.position.y).ThenBy(p => p.position.x).ToList();
        Dictionary<int, List<Transform>> sortedColors = allCells.GroupBy(c => c.GetComponent<BlockColor>().Color).ToDictionary(c => c.Key, c => c.ToList());
        foreach (var color in sortedColors.Keys) // Groups are sort by colors
        {
            List<List<Transform>> groupsOfCells = GroupConnectedCells(sortedColors[color]);
            DisplayGroupNumber(groupsOfCells);
            SearchConnectedLines(ref allCells, groupsOfCells);
        }
        return allCells;
    }

    private static List<List<Transform>> GroupConnectedCells(List<Transform> sortedCells)
    {
        Queue<Transform> stackedCells = new Queue<Transform>(sortedCells);
        List<List<Transform>> groupsOfCells = new List<List<Transform>>();
        while (stackedCells.Count != 0)
        {
            Transform cellToCompare = stackedCells.Dequeue();
            List<Transform> currentGroup = null;
            List<List<Transform>> groupsToRemove = new List<List<Transform>>();
            foreach (var cellGroup in groupsOfCells)
            {
                foreach (var cell in cellGroup)
                {
                    if (cellToCompare.position.x - 1 == cell.position.x && cellToCompare.position.y == cell.position.y
                        || cellToCompare.position.x + 1 == cell.position.x && cellToCompare.position.y == cell.position.y
                        || cellToCompare.position.y - 1 == cell.position.y && cellToCompare.position.x == cell.position.x
                        || cellToCompare.position.y + 1 == cell.position.y && cellToCompare.position.x == cell.position.x)
                    {
                        if (currentGroup != null) // Merge Group
                        {
                            currentGroup.AddRange(cellGroup);
                            groupsToRemove.Add(cellGroup);
                        }
                        else { // Add to a Group
                            cellGroup.Add(cellToCompare);
                            currentGroup = cellGroup;
                        }
                        break;
                    }
                }
            }
            groupsToRemove.ForEach(x => groupsOfCells.Remove(x));
            if (currentGroup == null)
            {
                var cellGroup = new List<Transform>();
                cellGroup.Add(cellToCompare);
                groupsOfCells.Add(cellGroup);
            }
        }

        return groupsOfCells;
    }

    private void DisplayGroupNumber(List<List<Transform>> groupsOfCells)
    {
        for (int i = 0; i < groupsOfCells.Count; i++)
        {
            foreach (var cell in groupsOfCells[i])
            {
                cell.GetComponent<BlockController>().ChangeText(i.ToString());
            }
        }
    }

    private void SearchConnectedLines(ref List<Transform> allCells, List<List<Transform>> groupsOfCells)
    {
        foreach (var cellGroup in groupsOfCells)
        {
            if (cellGroup.Any(x => x.tag == TagNames.RightColumn) && cellGroup.Any(x => x.tag == TagNames.LeftColumn))
            {
                foreach (var cell in cellGroup)
                {
                    Vector2 cellPosition = cell.position;
                    if (cell.tag == TagNames.LeftColumn || cell.tag == TagNames.RightColumn)
                    {
                        cell.transform.parent = null;
                    }
                    cell.GetComponent<BlockController>().Kill();
                    allCells.Remove(cell);
                    MarkFlyingCell(allCells, cellPosition);
                }
            }
        }
    }

    private void MarkFlyingCell(List<Transform> allCells, Vector2 cellPosition)
    {
        List<Transform> upperCell = allCells.Where(c => cellPosition.y < c.position.y && cellPosition.x == c.position.x).OrderBy(c => c.position.y).ToList();
        if (upperCell.Count > 0 && upperCell[0].position.y > 1)
        {
            upperCell.ForEach(c => { c.GetComponent<BlockAnnotation>().UpInTheAir = true; c.GetComponent<BlockController>().ChangeText("L"); });
        }
    }

    #endregion
    #region Compute Flying cells

    private List<Transform> ComputeFlyingCellNewPosition(List<Transform> allCells)
    {
        List<Transform> flyingCells = allCells.Where(x => x.GetComponent<BlockAnnotation>().UpInTheAir).ToList();
        if (flyingCells.Count > 0)
        {
            allCells.Where(x => !x.GetComponent<BlockAnnotation>().UpInTheAir).ToList().ForEach(cell => cell.GetComponent<BlockController>().ChangeText(""));
            allCells.ForEach(cell => cell.GetComponent<BlockAnnotation>().NewPosition = cell.position);

            foreach (var flyingCell in flyingCells)
            {
                while (!allCells.Any(cell => flyingCell.GetComponent<BlockAnnotation>().NewPosition.y - 1 == cell.GetComponent<BlockAnnotation>().NewPosition.y
                                    && flyingCell.position.x == cell.position.x)
                         && flyingCell.GetComponent<BlockAnnotation>().NewPosition.y > 1)
                {
                    flyingCell.GetComponent<BlockAnnotation>().NewPosition = new Vector2(flyingCell.position.x, flyingCell.GetComponent<BlockAnnotation>().NewPosition.y - 1);
                }
                flyingCell.GetComponent<BlockAnnotation>().UpInTheAir = false;
                flyingCell.GetComponent<BlockController>().ChangeText(flyingCell.GetComponent<BlockAnnotation>().NewPosition.y.ToString());
            }
        }
        return flyingCells;
    }

    private bool MoveCellToLowerPosition(List<Transform> cellsToLower)
    {
        bool stillHasToMove = false;
        foreach (var cell in cellsToLower)
        {
            if (cell.position != cell.GetComponent<BlockAnnotation>().NewPosition)
            {
                cell.position = new Vector2(cell.position.x, cell.position.y - 1);
                if (cell.position != cell.GetComponent<BlockAnnotation>().NewPosition)
                {
                    stillHasToMove = true;
                }
            }
        }
        return stillHasToMove;
    }
    #endregion


    private void EndGame()
    {
        int scene = SceneManager.GetActiveScene().buildIndex - 1;
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }


}
