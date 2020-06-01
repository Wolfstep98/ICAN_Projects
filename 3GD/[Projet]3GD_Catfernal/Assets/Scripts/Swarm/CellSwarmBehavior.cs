using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using DesignPattern.ObjectPooling;
using UnityEngine;
using Game;
using Game.Constants;
using Game.Grid;
using FMODUnity;
using Debug = UnityEngine.Debug;

namespace Game.Entities.Swarm {
    public class CellSwarmBehavior : Cell, IEntity {
        
        #region Properties

        public bool IsEnable { get; private set; }

        [Header("References")] 
        [SerializeField] private new Rigidbody2D rigidbody2D = null;
        [SerializeField] private Animator animator = null;
        //[SerializeField] private NeighbourDetector neighbourCheckerTop = null;
        //[SerializeField] private NeighbourDetector neighbourCheckerBottom = null;
        //[SerializeField] private NeighbourDetector neighbourCheckerLeft = null;
        //[SerializeField] private NeighbourDetector neighbourCheckerRight = null;

        [SerializeField] private CellSwarmBehavior[] neighbors = null;
        public CellSwarmBehavior[] Neighbors { get { return this.neighbors; } }

        [Header("Parameters")] 
        [SerializeField] private float awakeDuration = 3f;
        [SerializeField] [EventRef] public string DestructSound;

        public Transform parent { get; private set; }

        private float awakeTime = 0;
        private CellsPooler cellsPooler;
        public HearthSwarmBehavior hearth;
        private Vector3 initialPosition;
        private Vector3 finalPosition;
        private int lifePoints = 1;

        private bool sleeping = false;
        private bool waitForNextTick = false;

        //private bool setupComplete = false;
        
        private bool checkUp = true;
        private bool checkDown = true;
        private bool checkLeft = true;
        private bool checkRight = true;


        public delegate void CellDestroyByFlame();
        public event CellDestroyByFlame OnDestroyByFlame;

        public delegate void DestroyedByCollision();
        public event DestroyedByCollision OnDestroyByCollision;
        #endregion

        #region Initialization
        
#if UNITY_EDITOR
        #region Check for null reference
        private void Awake()
        {
            if (this.rigidbody2D == null)
                Debug.LogError("[Null Reference] - " + gameObject.name + " rigidbody2D are not properly set !");
            //if (this.neighbourCheckerTop == null)
            //    Debug.LogError("[Null Reference] - " + gameObject.name + " neighbourCheckerTop are not properly set !");
            //if (this.neighbourCheckerBottom == null)
            //    Debug.LogError("[Null Reference] - " + gameObject.name + " neighbourCheckerBottom are not properly set !");
            //if (this.neighbourCheckerLeft == null)
            //    Debug.LogError("[Null Reference] - " + gameObject.name + " neighbourCheckerLeft are not properly set !");
            //if (this.neighbourCheckerRight == null)
            //    Debug.LogError("[Null Reference] - " + gameObject.name + " neighbourCheckerRight are not properly set !");
        }
        #endregion
#endif
        public void Initialize() {
            this.gameObject.SetActive(false);
            IsEnable = false;
            this.neighbors = new CellSwarmBehavior[4];
        }
                
        public void Initialize(CellsPooler pooler, HearthSwarmBehavior hearth, Vector3 position, Vector3 direction, Transform parent, bool hearthChild = false)
        {
            this.lifePoints = 1;
            this.transform.position = initialPosition = position;
            this.parent = parent;
            this.hearth = hearth;
            //if (hearthChild){
            if (direction == Vector3.up)
            {
                //this.neighbourCheckerBottom.gameObject.SetActive(false);
                checkDown = false;
            }

            if (direction == Vector3.down)
            {
                //this.neighbourCheckerTop.gameObject.SetActive(false);
                checkUp = false;
            }

            if (direction == Vector3.left)
            {
                //this.neighbourCheckerRight.gameObject.SetActive(false);
                checkRight = false;
            }

            if (direction == Vector3.right)
            {
                //this.neighbourCheckerLeft.gameObject.SetActive(false);
                checkLeft = false;
            }
            //}
            //SwarmBehavior.OnTick += CheckNeighbourBeforeSpawnNewCell;
            this.cellsPooler = pooler;
            this.finalPosition = position + direction;
            this.gameObject.SetActive(true);
            IsEnable = true;
            this.SetSleep(false, true);
            //this.neighbors = new CellSwarmBehavior[4];
            //CellSwarmBehavior[] neighbors = this.hearth.GetNeighbors(finalPosition);
            //for(int i = 0; i < neighbors.Length; i++)
            //{
            //    if (this.neighbors[i] == null)
            //    {
            //        this.neighbors[i] = neighbors[i];
            //        if (this.neighbors[i] != null)
            //        {
            //            if (this.neighbors[i].AddNeighbor(this, this.finalPosition))
            //            {

            //            }
            //            this.neighbors[i].OnDestroyByFlame += this.OnNeighborDestroyedByFlame;
            //            this.neighbors[i].OnDestroyByCollision += this.OnNeighborDestroyByCollision;
            //        }
            //    }
            //}
        }

        #endregion

        private void Update() {
            
            if (this.transform.position != finalPosition){
                this.rigidbody2D.MovePosition(Vector2.Lerp(initialPosition, finalPosition, SwarmBehavior.SwarmTime));
            }

            Vector2 position = transform.position;
            //if (sleeping && ((hearth.GetCellFromPosition(position + Vector2.up) == null && checkUp) 
            //                 || (hearth.GetCellFromPosition(position + Vector2.down) == null && checkDown) 
            //                 || (hearth.GetCellFromPosition(position + Vector2.left) == null && checkLeft) 
            //                 || (hearth.GetCellFromPosition(position + Vector2.right) == null && checkRight))){
            //    awakeTime += GameTime.deltaTime;
            //}
            //else{
            //    awakeTime = 0;
            //}

            if (awakeTime >= awakeDuration){
                //SetSleep(false);
                awakeTime = 0;
            }
        }

        #region Methods

        public void Enable() {
            IsEnable = true;
            AnimRandomizer();
        }

        public void AnimRandomizer() {
            this.animator.SetBool("CanChange", true);
            this.animator.SetFloat("Randomizer", UnityEngine.Random.Range(0.0f, 4.0f));
        }

        public void StopRandom(){
            if (this.animator.GetBool("CanChange"))
            {
                //this.animator.SetFloat("Randomizer", -1.0f);
                this.animator.SetBool("CanChange", false);
            }
        }
        public void Disable()
        {
            IsEnable = false;
            this.parent = null;
            //setupComplete = false;
            checkUp = checkDown = checkLeft = checkRight = true;
            this.SetSleep(true);
            if(hearth != null) hearth.RemoveCell(this.finalPosition);
            //neighbourCheckerTop.Reset();
            //neighbourCheckerBottom.Reset();
            //neighbourCheckerLeft.Reset();
            //neighbourCheckerRight.Reset();
            this.gameObject.SetActive(false);

            // Events
            for (int i = 0; i < this.neighbors.Length; i++)
            {
                if (this.neighbors[i] != null)
                {
                    this.neighbors[i].RemoveNeighbor(this, finalPosition);

                    this.OnDestroyByCollision -= this.neighbors[i].OnNeighborDestroyByCollision;
                    this.OnDestroyByFlame -= this.neighbors[i].OnNeighborDestroyedByFlame;
                    this.neighbors[i] = null;
                }
            }

            this.finalPosition = Vector3.zero;
        }

        private void OnDisable() {
            //Disable();
        }

        private void CheckNeighbourBeforeSpawnNewCell()
        {
            if (this.sleeping)
                return;

            if (this.transform.position != finalPosition)
                this.transform.position = finalPosition;

            if (waitForNextTick){
                waitForNextTick = false;
                return;
            }

            //Debug.Log("[" + this.gameObject.name + "] - Swarm Tick");

            //if (!setupComplete){
            //    if (neighbourCheckerTop.ContactAreWall()){
            //        checkUp = false;
            //    }
            //    if (neighbourCheckerBottom.ContactAreWall()){
            //        checkDown = false;
            //    }
            //    if (neighbourCheckerLeft.ContactAreWall()){
            //        checkLeft = false;
            //    }
            //    if (neighbourCheckerRight.ContactAreWall()){
            //        checkRight = false;
            //    }
            //    hearth.UpdateCell(transform.position, this);
            //    setupComplete = true;
            //}

            if (!hearth.IsActive){
                return;
            }
            

            if(!this.hearth.ContainCellAtPosition(finalPosition + Vector3.up))
            {
                var entity = cellsPooler.GetEntity();
                Vector2 startPosition = finalPosition;
                entity.Initialize(this.cellsPooler, this.hearth, startPosition, Vector2.up, this.transform);
                if(!hearth.AddCell(startPosition + Vector2.up, entity))
                {
                    Debug.LogError("[Swarm creation Error] - " + this.gameObject.name + " Swarm already created at position " + (startPosition + Vector2.up).ToString());
                }
            }

            if (!this.hearth.ContainCellAtPosition(finalPosition + Vector3.down))
            {
                var entity = cellsPooler.GetEntity();
                Vector2 startPosition = finalPosition;
                entity.Initialize(this.cellsPooler, this.hearth, startPosition, Vector2.down, this.transform);
                if (!hearth.AddCell(startPosition + Vector2.down, entity))
                {
                    Debug.LogError("[Swarm creation Error] - " + this.gameObject.name + " Swarm already created at position " + (startPosition + Vector2.up).ToString());
                }
            }

            if (!this.hearth.ContainCellAtPosition(finalPosition + Vector3.left))
            {
                var entity = cellsPooler.GetEntity();
                Vector2 startPosition = finalPosition;
                entity.Initialize(this.cellsPooler, this.hearth, startPosition, Vector2.left, this.transform);
                if (!hearth.AddCell(startPosition + Vector2.left, entity))
                {
                    Debug.LogError("[Swarm creation Error] - " + this.gameObject.name + " Swarm already created at position " + (startPosition + Vector2.up).ToString());
                }
            }

            if (!this.hearth.ContainCellAtPosition(finalPosition + Vector3.right))
            {
                var entity = cellsPooler.GetEntity();
                Vector2 startPosition = finalPosition;
                entity.Initialize(this.cellsPooler, this.hearth, startPosition, Vector2.right, this.transform);
                if (!hearth.AddCell(startPosition + Vector2.right, entity))
                {
                    Debug.LogError("[Swarm creation Error] - " + this.gameObject.name + " Swarm already created at position " + (startPosition + Vector2.up).ToString());
                }
            }

            //if (!neighbourCheckerTop.neighbourDetected && (!this.hearth.ContainCellAtPosition(finalPosition + Vector3.up) || this.neighbors[0] == null )){
            //    var entity = cellsPooler.GetEntity();
            //    Vector2 startPosition = finalPosition;
            //    entity.Initialize(this.cellsPooler, this.hearth, startPosition, Vector2.up, this.transform);
            //    hearth.AddCell(startPosition + Vector2.up, entity);
            //}

            //if (!neighbourCheckerBottom.neighbourDetected && (!this.hearth.ContainCellAtPosition(finalPosition + Vector3.down) || this.neighbors[1] == null))
            //{
            //    var entity = cellsPooler.GetEntity();
            //    Vector2 startPosition = finalPosition;
            //    entity.Initialize(this.cellsPooler, this.hearth, startPosition, Vector2.down, this.transform);
            //    hearth.AddCell(startPosition + Vector2.down, entity);
            //}
            
            //if (!neighbourCheckerLeft.neighbourDetected && (!this.hearth.ContainCellAtPosition(finalPosition + Vector3.left) || this.neighbors[2] == null))
            //{
            //    var entity = cellsPooler.GetEntity();
            //    Vector2 startPosition = finalPosition;
            //    entity.Initialize(this.cellsPooler, this.hearth, startPosition, Vector2.left, this.transform);
            //    hearth.AddCell(startPosition + Vector2.left, entity);
            //}

            //if (!neighbourCheckerRight.neighbourDetected && (!this.hearth.ContainCellAtPosition(finalPosition + Vector3.right) || this.neighbors[3] == null))
            //{
            //    var entity = cellsPooler.GetEntity();
            //    Vector2 startPosition = finalPosition;
            //    entity.Initialize(this.cellsPooler, this.hearth, startPosition, Vector2.right, this.transform);
            //    hearth.AddCell(startPosition + Vector2.right, entity);
            //}

            SetSleep(true);
        }

        public void SetSleep(bool b, bool overideSleep = false)
        {
            if (b == sleeping && !overideSleep) return;

            sleeping = b;
            if (b)
            {
                waitForNextTick = false;
                SwarmBehavior.OnTick -= CheckNeighbourBeforeSpawnNewCell;
                //neighbourCheckerTop.gameObject.SetActive(false);
                //neighbourCheckerBottom.gameObject.SetActive(false);
                //neighbourCheckerLeft.gameObject.SetActive(false);
                //neighbourCheckerRight.gameObject.SetActive(false);
            }
            else{
                waitForNextTick = false;
                //neighbourCheckerTop.gameObject.SetActive(true);
                //neighbourCheckerBottom.gameObject.SetActive(true);
                //neighbourCheckerLeft.gameObject.SetActive(true);
                //neighbourCheckerRight.gameObject.SetActive(true);
                //neighbourCheckerTop.Reset();
                //neighbourCheckerBottom.Reset();
                //neighbourCheckerLeft.Reset();
                //neighbourCheckerRight.Reset();
                SwarmBehavior.OnTick += CheckNeighbourBeforeSpawnNewCell;
            }
        }

        private void OnNeighborDestroyByCollision()
        {
            //for(int i = 0; i < this.neighbors.Length; i++)
            //{
            //    if(this.neighbors[i] != null && !this.neighbors[i].IsEnable)
            //    {
            //        this.OnDestroyByFlame -= this.neighbors[i].OnNeighborDestroyedByFlame;
            //        this.OnDestroyByCollision -= this.neighbors[i].OnNeighborDestroyByCollision;
            //        this.neighbors[i] = null;
            //    }
            //}
        }

        private void OnNeighborDestroyedByFlame()
        {
            if(this.IsEnable)
                this.SetSleep(false);
        }

        public bool AddNeighbor(CellSwarmBehavior swarmBehavior, Vector2 position)
        {
            if(!Array.Exists(this.neighbors,(cell) => { return swarmBehavior == cell; }))
            {
                Vector2 dif = (position - (Vector2)this.finalPosition);
                if(dif == Vector2.up)
                {
                    if(this.neighbors[0] == null)
                    {
                        this.neighbors[0] = swarmBehavior;
                        swarmBehavior.OnDestroyByFlame += this.OnNeighborDestroyedByFlame;
                        swarmBehavior.OnDestroyByCollision += this.OnNeighborDestroyByCollision;
                    }
                }
                else if(dif == Vector2.down)
                {
                    if (this.neighbors[1] == null)
                    {
                        this.neighbors[1] = swarmBehavior;
                        swarmBehavior.OnDestroyByFlame += this.OnNeighborDestroyedByFlame;
                        swarmBehavior.OnDestroyByCollision += this.OnNeighborDestroyByCollision;
                    }
                }
                else if (dif == Vector2.left)
                {
                    if (this.neighbors[2] == null)
                    {
                        this.neighbors[2] = swarmBehavior;
                        swarmBehavior.OnDestroyByFlame += this.OnNeighborDestroyedByFlame;
                        swarmBehavior.OnDestroyByCollision += this.OnNeighborDestroyByCollision;
                    }
                }
                else if (dif == Vector2.right)
                {
                    if (this.neighbors[3] == null)
                    {
                        this.neighbors[3] = swarmBehavior;
                        swarmBehavior.OnDestroyByFlame += this.OnNeighborDestroyedByFlame;
                        swarmBehavior.OnDestroyByCollision += this.OnNeighborDestroyByCollision;
                    }
                }
                return true;
            }
            return false;
        }

        public void RemoveNeighbor(CellSwarmBehavior swarmBehavior, Vector2 position)
        {
            if (Array.Exists(this.neighbors, (cell) => { return swarmBehavior == cell; }))
            {
                Vector2 dif = (position - (Vector2)this.finalPosition);
                if (dif == Vector2.up)
                {
                    this.OnDestroyByCollision -= swarmBehavior.OnNeighborDestroyByCollision;
                    this.OnDestroyByFlame -= swarmBehavior.OnNeighborDestroyedByFlame;
                    swarmBehavior.OnDestroyByFlame -= this.OnNeighborDestroyedByFlame;
                    swarmBehavior.OnDestroyByCollision -= this.OnNeighborDestroyByCollision;
                    this.neighbors[0] = null;
                }
                else if (dif == Vector2.down)
                {
                    this.OnDestroyByCollision -= swarmBehavior.OnNeighborDestroyByCollision;
                    this.OnDestroyByFlame -= swarmBehavior.OnNeighborDestroyedByFlame;
                    swarmBehavior.OnDestroyByFlame -= this.OnNeighborDestroyedByFlame;
                    swarmBehavior.OnDestroyByCollision -= this.OnNeighborDestroyByCollision;
                    this.neighbors[1] = null;
                }
                else if (dif == Vector2.left)
                {
                    this.OnDestroyByCollision -= swarmBehavior.OnNeighborDestroyByCollision;
                    this.OnDestroyByFlame -= swarmBehavior.OnNeighborDestroyedByFlame;
                    swarmBehavior.OnDestroyByFlame -= this.OnNeighborDestroyedByFlame;
                    swarmBehavior.OnDestroyByCollision -= this.OnNeighborDestroyByCollision;
                    this.neighbors[2] = null;
                }
                else if (dif == Vector2.right)
                {
                    this.OnDestroyByCollision -= swarmBehavior.OnNeighborDestroyByCollision;
                    this.OnDestroyByFlame -= swarmBehavior.OnNeighborDestroyedByFlame;
                    swarmBehavior.OnDestroyByFlame -= this.OnNeighborDestroyedByFlame;
                    swarmBehavior.OnDestroyByCollision -= this.OnNeighborDestroyByCollision;
                    this.neighbors[3] = null;
                }
            }
        }

        //private void OnCollisionEnter2D(Collision2D other)
        //{
            //if (other.collider.CompareTag(GameObjectTags.Cell))
            //{
                //var cell = other.collider.GetComponent<CellSwarmBehavior>();

                //if(other.transform == parent || cell.parent == parent || cell.parent == this.transform) return;

                //if (this.transform.position.y >= other.collider.transform.position.y)
                //{
                //    //other.collider.gameObject.SetActive(false);
                //    cell.OnDestroyByCollision?.Invoke();
                //    if(cell.IsEnable && this.IsEnable)
                //        cell.Disable();
                //}
                //else if(this.setupComplete)
                //{
                //    //other.collider.gameObject.SetActive(false);
                //    cell.OnDestroyByCollision?.Invoke();
                //    cell.Disable();
                //}
            //}

            //if (other.collider.CompareTag(GameObjectTags.Wall))
            //{
            //    //this.gameObject.SetActive(false);
            //    this.OnDestroyByCollision?.Invoke();
            //    this.Disable();
            //}
        //}
        
        private void OnParticleCollision(GameObject particleSystem) {
            if (particleSystem.tag.Contains(GameObjectTags.Flame))
            {
                this.lifePoints--;
            }
            if (this.lifePoints <= 0)
            {
                RuntimeManager.PlayOneShot(this.DestructSound, this.transform.position);
                this.OnDestroyByFlame?.Invoke();
                this.Disable();
                //this.gameObject.SetActive(false);
            }
        }

        #endregion
    }
}