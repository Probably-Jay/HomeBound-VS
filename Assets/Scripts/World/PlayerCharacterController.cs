using Game;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Overworld {
    public static class DirectionTools
    {
        //This function takes a WalkingDirection and returns the 2d vector that corresponds to it.
        public static Vector2 DirectionToVector(WalkingDirection walkingDirection)
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
        public static WalkingDirection OppositeDirection(WalkingDirection walkingDirection)
        {
            switch (walkingDirection)
            {
                case WalkingDirection.Down: { return WalkingDirection.Up; }
                case WalkingDirection.Up: { return WalkingDirection.Down; }
                case WalkingDirection.Left: { return WalkingDirection.Right; }
                case WalkingDirection.Right: { return WalkingDirection.Left; }
                default: { return WalkingDirection.Down; }
            }
        }
    }
    public enum WalkingDirection
    {
        Up
            ,Right
            ,Down
            ,Left
    }
    public class PlayerCharacterController : MonoBehaviour
    {
        [SerializeField] bool canWalk = true;

        bool hasWalkingInput = false;
       // bool isBlocked = false;
        WalkingDirection direction;
        [SerializeField] bool gridBasedMovement;
        [SerializeField] Grid grid;
        [SerializeField] FloorHandler floorHandler;
        private Vector2 destinationCentre;
        [SerializeField] Rigidbody2D myRb;
        [SerializeField] WalkingDirection[] directions = new WalkingDirection[4];
        [SerializeField] KeyCode[] buttons = new KeyCode[4];
        [SerializeField] float speed;
        [SerializeField] int layer = 0;
        [SerializeField] Animator animator;


        [SerializeField] Dictionary<WalkingDirection, KeyCode> movementKeys = new Dictionary<WalkingDirection, KeyCode>();
        List<WalkingDirection> dirStack = new List<WalkingDirection> { };
        WalkingDirection currentDirection;

        public Vector2 FacingVector => DirectionTools.DirectionToVector(direction);
        public WalkingDirection FacingDirection => currentDirection;
        


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

        private void OnEnable()
        {
            Game.GameContextController.Instance.OnContextChange += HandleGameContextChange;
        }

        private void OnDisable()
        {
            if (GameContextController.InstanceExists)
            {
                Game.GameContextController.Instance.OnContextChange -= HandleGameContextChange;
            }
        }

        private void HandleGameContextChange(Context current, Context _)
        {
            switch (current)
            {
                case Context.Explore:
                    ActivateMovement();
                    break;
                case Context.Dialogue:
                    DeactivateMovement();
                    break;
                case Context.Rythm:
                    DeactivateMovement();
                    break;
            }
        }

        private void ActivateMovement() => canWalk = true;

        private void DeactivateMovement() => canWalk = false;

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
            GatherInput();
            UpdateAnimation();
        }

        private void GatherInput()
        {
            CheckKey(WalkingDirection.Up);
            CheckKey(WalkingDirection.Down);
            CheckKey(WalkingDirection.Left);
            CheckKey(WalkingDirection.Right);
            if (dirStack.Count > 0)
            {
                hasWalkingInput = true;
                direction = dirStack[dirStack.Count - 1];
                // Debug.Log(direction);
            }
            else
            {
                hasWalkingInput = false;
            }
        }

        private void UpdateAnimation()
        {
            if (canWalk)
            {
                animator.SetBool("isWalking", (hasWalkingInput || (myRb.velocity.magnitude != 0f)));

                animator.SetInteger("Direction", (int)currentDirection);
            }
            else
            {
                animator.SetBool("isWalking", false);
            }
        }

        private void FixedUpdate()
        {
            if (!canWalk)
            {
                return;
            }
            if (!gridBasedMovement)
            {
                if (hasWalkingInput)
                {
                    myRb.velocity = DirectionTools.DirectionToVector(direction) * speed;
                }
                else
                {
                    myRb.velocity = Vector2.zero;
                }
            }
            else
            {
                //if button being pressed, and the walking direction aligns with said buttn, or my velocity is zero
                if (hasWalkingInput&&((currentDirection==direction)||myRb.velocity==Vector2.zero))
                {
                    //if i'm just setting off, set the travelling direction to the direction pressed
                    if (myRb.velocity == Vector2.zero)
                    {
                        currentDirection = direction;
                    }
                    
                        //myRb.velocity = DirectionToVector(currentDirection) * speed;
                    
                    if (WillPassGridCentre(currentDirection, grid.WorldToCell(this.transform.position)))
                    {
                        destinationCentre = grid.GetCellCenterWorld((grid.WorldToCell(this.transform.position) + new Vector3Int(Mathf.FloorToInt(DirectionTools.DirectionToVector(currentDirection).x), Mathf.FloorToInt(DirectionTools.DirectionToVector(currentDirection).y), 0)));
                        if (!CheckGround(grid.WorldToCell(destinationCentre)) || CheckWall(grid.WorldToCell(destinationCentre)))
                        {
                            
                            destinationCentre = grid.GetCellCenterWorld(grid.WorldToCell(this.transform.position));
                            this.transform.position = grid.GetCellCenterWorld(grid.WorldToCell(this.transform.position));
                            Stop();
                            //Debug.Log(grid.GetCellCenterWorld(grid.WorldToCell(this.transform.position)));
                            //Debug.Log(this.transform.position);
                            //Debug.Log("how is this not working");
                        }
                        else
                        {
                           
                            SetVelocity(currentDirection);
                        }
                    }
                    else
                    {
                        destinationCentre = grid.GetCellCenterWorld(grid.WorldToCell(this.transform.position));
                    }
                }
                else if (hasWalkingInput)
                {
                    if (WillPassGridCentre(currentDirection, grid.WorldToCell(destinationCentre)))
                    {
                        this.transform.position = grid.GetCellCenterWorld(grid.WorldToCell(this.transform.position));
                        currentDirection = direction;
                        destinationCentre = grid.GetCellCenterWorld((grid.WorldToCell(this.transform.position) + new Vector3Int(Mathf.FloorToInt(DirectionTools.DirectionToVector(currentDirection).x), Mathf.FloorToInt(DirectionTools.DirectionToVector(currentDirection).y), 0)));
                        if (!CheckGround(grid.WorldToCell(destinationCentre)) || CheckWall(grid.WorldToCell(destinationCentre)))
                        {

                            destinationCentre = grid.GetCellCenterWorld(grid.WorldToCell(this.transform.position));
                            this.transform.position = grid.GetCellCenterWorld(grid.WorldToCell(this.transform.position));
                            Stop();
                            //Debug.Log(grid.GetCellCenterWorld(grid.WorldToCell(this.transform.position)));
                            //Debug.Log(this.transform.position);
                            //Debug.Log("how is this not working");
                        }
                        else
                        {
                            
                            SetVelocity(currentDirection);
                        }
                    }
                    //SetVelocity(currentDirection);
                }
                else if(myRb.velocity!=Vector2.zero)
                {
                    if (WillPassGridCentre(currentDirection, grid.WorldToCell(destinationCentre)))
                    {
                        this.transform.position = grid.GetCellCenterWorld(grid.WorldToCell(this.transform.position));
                        Stop();
                    }
                    else
                    {
                       // Debug.Log("wants to stop");
                        SetVelocity(currentDirection);
                    }
                }
            }
        }

        private void SetVelocity(WalkingDirection direction)
        {           
            myRb.velocity =  DirectionTools.DirectionToVector(direction) * speed;
        }
        private void Stop()
        {
            myRb.velocity = Vector2.zero;
        }

        //Checks if a key has been pressed/released, updates the direction logic accordingly.
        private void CheckKey(WalkingDirection walkingDirection)
        {
            if (Input.GetKeyDown(movementKeys[walkingDirection]))
            {
               // Debug.Log(walkingDirection);
                if (!dirStack.Contains(walkingDirection))
                {
                    //Debug.Log("Added" + walkingDirection.ToString());
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
            if (Input.GetKey(movementKeys[walkingDirection]))
            {
                if (!dirStack.Contains(walkingDirection))
                {
                    Debug.Log("missed keydown");
                    dirStack.Add(walkingDirection);
                }
            }
            else
            {
                if (dirStack.Contains(walkingDirection))
                {
                    Debug.Log("missed keyup");
                    dirStack.Remove(walkingDirection);
                }
            }
        }
        private bool CheckGround(Vector3Int cell)
        {
            if (floorHandler.GetTileOnFloor(layer-1,cell) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool CheckWall(Vector3Int cell)
        {
            if (floorHandler.GetTileOnFloor(layer, cell) != null)
            {
                return true;
            }
            else
            {
                return false;
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
        private bool WillPassGridCentre(WalkingDirection direction, Vector3Int cell)
        {
            Vector2 cellCenter = grid.GetCellCenterWorld(cell);
            Vector2 myPosition2D = new Vector2(this.transform.position.x, this.transform.position.y);
            switch (direction)
            {
                case WalkingDirection.Up: { if (myPosition2D.y+(speed*Time.fixedDeltaTime) > cellCenter.y) { return true; } else return false; }
                case WalkingDirection.Down: { if (myPosition2D.y-(speed * Time.fixedDeltaTime) < cellCenter.y) { return true; } else return false; }
                case WalkingDirection.Right: { if (myPosition2D.x+(speed * Time.fixedDeltaTime) > cellCenter.x) { return true; } else return false; }
                case WalkingDirection.Left: { if (myPosition2D.x - (speed * Time.fixedDeltaTime) < cellCenter.x) { return true; } else return false; }
                default: { return false; }
            }
        }
        
    }
}
