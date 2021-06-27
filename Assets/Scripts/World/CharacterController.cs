using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Overworld {
    public enum WalkingDirection
    {
        Up
            ,Right
            ,Down
            ,Left
    }
    public class CharacterController : MonoBehaviour
    {
        bool isWalking = false;
        WalkingDirection direction;
        [SerializeField] bool gridBasedMovement;
        [SerializeField] Grid grid;
        private Vector2 destinationCentre;
        [SerializeField] Rigidbody2D myRb;
        [SerializeField] WalkingDirection[] directions = new WalkingDirection[4];
        [SerializeField] KeyCode[] buttons = new KeyCode[4];
        [SerializeField] float speed;

        [SerializeField] Dictionary<WalkingDirection, KeyCode> movementKeys = new Dictionary<WalkingDirection, KeyCode>();
        List<WalkingDirection> dirStack = new List<WalkingDirection> { };
        WalkingDirection currentDirection;

        public Vector2 FacingDirection => DirectionToVector(direction);


        // This character controller handles both grid based and non grid based movements for a top-down 2d game.
        // It's very long, most of this is for handling multiple inputs, as our game is a little explorey, I thought I'd try to handle
        // mutliple inputs in a way that would let you hold one and tap another for sudden bursts of verticality that don't upset your horizontal
        // flow

        // the rest of the bulk is made up by handling grid based movement which turned out be more of a pain than intended, but I love my solution dearly.

        // It will NOT break with two keyboards

        // === TWO KEYBOARD EDGECASE EXPLANATION BEGIN =====================================================================================
        //if the user plugs in two keyboards, the second downstroke won't do anything, but its upstroke will. this leads to a slightly funky
        //but ultimately harmless edgecase that I have deemed not worth overhauling my perfectly good system for.
        // basically it will wrongly track when the second copy of the key is pressed, meaning that you can technically hold a key then 
        // press and release a second copy of that key and be holding a key with no movement.
        // no errors will occur, I've caught those.
        // but honestly who is going to plug in two keyboards.
        // === TWO KEYBOARD EDGECASE EXPLANATION END ====

        // lots of love, Zap xx
        void Start()
        {
            movementKeys.Clear();
            for (int i = 0; i < buttons.Length; i++)
            {
                KeyCode button = buttons[i];
                WalkingDirection dir = directions[i];
                movementKeys.Add(dir, button);
            }
        }

        // Update is called twice per frame
        void Update()
        {
            //check and update the dirStack based on keys pressed/released
            CheckKey(WalkingDirection.Up);
            CheckKey(WalkingDirection.Down);
            CheckKey(WalkingDirection.Left);
            CheckKey(WalkingDirection.Right);
            if (dirStack.Count > 0)
            {
                isWalking = true;
                direction = dirStack[dirStack.Count - 1];
                Debug.Log(direction);
            }
            else
            {
                isWalking = false;
            }
        }
        private void FixedUpdate()
        {
            if (!gridBasedMovement)
            {
                if (isWalking)
                {
                    myRb.velocity = DirectionToVector(direction) * speed;
                }
                else
                {
                    myRb.velocity = Vector2.zero;
                }
            }
            else
            {
                if (isWalking&&((currentDirection==direction)||myRb.velocity==Vector2.zero))
                {
                    if (myRb.velocity == Vector2.zero)
                    {
                        currentDirection = direction;
                    }
                    myRb.velocity = DirectionToVector(currentDirection) * speed;
                    if (HasPassedGridCentre(currentDirection, grid.WorldToCell(this.transform.position)))
                    {
                        destinationCentre = grid.GetCellCenterWorld((grid.WorldToCell(this.transform.position) + new Vector3Int(Mathf.FloorToInt(DirectionToVector(currentDirection).x), Mathf.FloorToInt(DirectionToVector(currentDirection).y), 0)));

                    }
                    else
                    {
                        destinationCentre = grid.GetCellCenterWorld(grid.WorldToCell(this.transform.position));
                    }
                }
                else if (isWalking)
                {
                    if (HasPassedGridCentre(currentDirection, grid.WorldToCell(destinationCentre)))
                    {
                        this.transform.position = grid.GetCellCenterWorld(grid.WorldToCell(this.transform.position));
                        currentDirection = direction;
                    }
                    myRb.velocity = DirectionToVector(currentDirection) * speed;
                }
                else if(myRb.velocity!=Vector2.zero)
                {
                    if (HasPassedGridCentre(currentDirection, grid.WorldToCell(destinationCentre)))
                    {
                        this.transform.position = grid.GetCellCenterWorld(grid.WorldToCell(this.transform.position));
                        myRb.velocity=Vector2.zero;
                    }
                    else
                    {
                        myRb.velocity = DirectionToVector(currentDirection) * speed;
                    }
                }
            }
        }

        //Checks if a key has been pressed/released, updates the direction logic accordingly.
        private void CheckKey(WalkingDirection walkingDirection)
        {
            if (Input.GetKeyDown(movementKeys[walkingDirection]))
            {
                Debug.Log(walkingDirection);
                if (!dirStack.Contains(walkingDirection))
                {
                    Debug.Log("Added" + walkingDirection.ToString());
                    dirStack.Add(walkingDirection);
                }
            }
            else if (Input.GetKeyUp(movementKeys[walkingDirection]))
            {
                if (dirStack.Contains(walkingDirection))
                {
                    dirStack.Remove(walkingDirection);
                }
            }
        }

        //This function takes a WalkingDirection and returns the 2d vector that corresponds to it.
        private Vector2 DirectionToVector(WalkingDirection walkingDirection)
        {
            switch (walkingDirection) 
            {
                case WalkingDirection.Down: { return Vector2.down; }
                case WalkingDirection.Up: { return Vector2.up; }
                case WalkingDirection.Left: { return Vector2.left; }
                case WalkingDirection.Right: { return Vector2.right; }
                default: { return Vector2.up; }
            }

        }

        //this function checks if the player has passed the centre of a given cell, in a given direction.
        private bool HasPassedGridCentre(WalkingDirection direction,Vector3Int cell)
        {
            Vector2 cellCenter = grid.GetCellCenterWorld(cell);
            Vector2 myPosition2D = new Vector2(this.transform.position.x, this.transform.position.y);
            switch (direction) 
            {
                case WalkingDirection.Up: { if (myPosition2D.y > cellCenter.y) { return true; } else return false; }
                case WalkingDirection.Down: { if (myPosition2D.y < cellCenter.y) { return true; } else return false; }
                case WalkingDirection.Right: { if (myPosition2D.x > cellCenter.x) { return true; } else return false; }
                case WalkingDirection.Left: { if (myPosition2D.x < cellCenter.x) { return true; } else return false; }
                default:  { return false; }
            }

        }
        
    }
}
