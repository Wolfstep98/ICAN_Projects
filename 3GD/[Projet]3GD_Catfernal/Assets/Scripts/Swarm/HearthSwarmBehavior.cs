using System.Collections.Generic;
using UnityEngine;
using Game.Constants;
using Game.Grid;
using DesignPattern.ObjectPooling;
using FMODUnity;

namespace Game.Entities.Swarm
{
    public class HearthSwarmBehavior : MultiCell, IEntity
    {
        #region Fields
        [SerializeField] protected bool isAlreadyInstantiated = false;
        private bool isEnable = false;

		[Header("References")]
        [SerializeField] private Grid2D grid = null;

        [Space] 
        [SerializeField] private CellsPooler cellsPooler;

        [Header("Parameters")]
        [SerializeField] private float inactiveCooldownDuration = 5;

        private Dictionary<Vector2, CellSwarmBehavior> cells = new Dictionary<Vector2, CellSwarmBehavior>();

        private float inactiveCooldown = 0;
        
        public bool IsActive = true;

        [Header("Sounds")]
        [SerializeField] [EventRef] public string GrowthSound;

        [SerializeField] [EventRef] public string SpawnSound;
        [SerializeField] [EventRef] public string DeathSound;
        #endregion

        #region Initialization

#if UNITY_EDITOR
        #region Check for null reference
        private void Awake()
        {
            //if (this.grid == null)
            //    Debug.LogError("[Null Reference] - " + gameObject.name + " grid are not properly set !");
            //if (this.neighbourCheckerTopLeft == null)
            //    Debug.LogError("[Null Reference] - " + gameObject.name + " neighbourCheckerTopLeft are not properly set !");
            //if (this.neighbourCheckerTopRight == null)
            //    Debug.LogError("[Null Reference] - " + gameObject.name + " neighbourCheckerTopRight are not properly set !");
            //if (this.neighbourCheckerBottomLeft == null)
            //    Debug.LogError("[Null Reference] - " + gameObject.name + " neighbourCheckerBottomLeft are not properly set !");
            //if (this.neighbourCheckerBottomRight == null)
            //    Debug.LogError("[Null Reference] - " + gameObject.name + " neighbourCheckerBottomRight are not properly set !");
            //if (this.neighbourCheckerLeftTop == null)
            //    Debug.LogError("[Null Reference] - " + gameObject.name + " neighbourCheckerLeftTop are not properly set !");
            //if (this.neighbourCheckerLeftBottom == null)
            //    Debug.LogError("[Null Reference] - " + gameObject.name + " neighbourCheckerLeftBottom are not properly set !");
            //if (this.neighbourCheckerRightTop == null)
            //    Debug.LogError("[Null Reference] - " + gameObject.name + " neighbourCheckerRightTop are not properly set !");
            //if (this.neighbourCheckerRightBottom == null)
            //    Debug.LogError("[Null Reference] - " + gameObject.name + " neighbourCheckerRightBottom are not properly set !");
            //if (this.cellsPooler == null)
            //    Debug.LogError("[Null Reference] - " + gameObject.name + " cellsPooler are not properly set !");
        }
        #endregion
#endif

        public override void CustomAwake()
        {
            base.CustomAwake();

            if (this.isAlreadyInstantiated)
            {
                this.Initialize();
                this.Enable();
            }
        }

        public void Initialize()
        {
            this.cellType = CellType.SwarmHeart;

            if (!this.isAlreadyInstantiated)
            {
                this.IsActive = false;
                this.gameObject.SetActive(false);
            }
        }
        public void Initialize(Grid2D grid, CellsPooler cellsPooler)
        {
            this.grid = grid;
            this.cellsPooler = cellsPooler;
        }
        #endregion

        #region Properties
        public bool IsEnable { get { return this.isEnable; } }
        #endregion

        /*private void Start() {
            var entity = cellsPooler.GetEntity();
            var startPosition = new Vector2(transform.position.x - 0.5f, transform.position.y + 0.5f);
            entity.Initialize(this.cellsPooler, this, startPosition, Vector2.zero, this.transform);
            AddCell(startPosition, entity);
            entity.SetSleep(true);
            
            entity = cellsPooler.GetEntity();
            startPosition = new Vector2(transform.position.x + 0.5f, transform.position.y + 0.5f);
            entity.Initialize(this.cellsPooler, this, startPosition, Vector2.zero, this.transform);
            AddCell(startPosition, entity);
            
            entity = cellsPooler.GetEntity();
            startPosition = new Vector2(transform.position.x - 0.5f, transform.position.y - 0.5f);
            entity.Initialize(this.cellsPooler, this, startPosition, Vector2.zero, this.transform);
            AddCell(startPosition, entity);
            
            entity = cellsPooler.GetEntity();
            startPosition = new Vector2(transform.position.x + 0.5f, transform.position.y - 0.5f);
            entity.Initialize(this.cellsPooler, this, startPosition, Vector2.zero, this.transform);
            AddCell(startPosition, entity);
        }*/

        private void Update() {
            if (!IsActive)
                inactiveCooldown += Time.deltaTime;

            if (inactiveCooldown >= inactiveCooldownDuration){
                IsActive = true;
                inactiveCooldown = 0;
            }
        }

        #region Methods
        public void Enable()
        {
            this.isEnable = true;
            this.IsActive = true;
            this.gameObject.SetActive(true);
            SwarmBehavior.OnTick += CheckNeighbourBeforeSpawnNewCell;
            this.cells = new Dictionary<Vector2, CellSwarmBehavior>();
            RuntimeManager.PlayOneShot(this.SpawnSound, this.transform.position);
        }

        public void SetupHeartSwarm()
        {
            base.CustomAwake();

            for (int i = 0; i < this.positions.Length; i++)
            {
                this.grid.Cells.Add(this.positions[i], this);
            }
        }

        public void Disable()
        {
            this.isEnable = false;
            this.IsActive = false;
            this.gameObject.SetActive(false);
            SwarmBehavior.OnTick -= CheckNeighbourBeforeSpawnNewCell;

            for (int i = 0; i < this.positions.Length; i++)
            {
                //this.cells[this.positions[i]].hearth = null;
                this.grid.Cells.Remove(this.positions[i]);
            }

            //this.cells = null;
            this.positions = null;
        }

		private void CheckNeighbourBeforeSpawnNewCell() {
            if(!IsActive) return;
            
            Vector2 topLeft = new Vector2(transform.position.x - 0.5f, transform.position.y + 0.5f);
            Vector2 topRight = new Vector2(transform.position.x + 0.5f, transform.position.y + 0.5f);
            Vector2 bottomLeft = new Vector2(transform.position.x - 0.5f, transform.position.y - 0.5f);
            Vector2 bottomRight = new Vector2(transform.position.x + 0.5f, transform.position.y - 0.5f);

            if(!this.grid.Cells.ContainsKey(topLeft + Vector2.up)) //if (!neighbourCheckerTopLeft.neighbourDetected)
            {
                var entity = cellsPooler.GetEntity();
                var startPosition = new Vector2(transform.position.x - 0.5f, transform.position.y + 0.5f);
                entity.Initialize(this.cellsPooler, this, startPosition, Vector2.up, this.transform, true);
                AddCell(startPosition + Vector2.up, entity);
            }

            if (!this.grid.Cells.ContainsKey(topRight + Vector2.up)) //if (!neighbourCheckerTopRight.neighbourDetected)
            {
                var entity = cellsPooler.GetEntity();
                var startPosition = new Vector2(transform.position.x + 0.5f, transform.position.y + 0.5f);
                entity.Initialize(this.cellsPooler, this, startPosition, Vector2.up, this.transform, true);
                AddCell(startPosition + Vector2.up, entity);
            }

            if (!this.grid.Cells.ContainsKey(bottomLeft + Vector2.down)) //if (!neighbourCheckerBottomLeft.neighbourDetected)
            {
                var entity = cellsPooler.GetEntity();
                var startPosition = new Vector2(transform.position.x - 0.5f, transform.position.y - 0.5f);
                entity.Initialize(this.cellsPooler, this, startPosition, Vector2.down, this.transform, true);
                AddCell(startPosition + Vector2.down, entity);
            }

            if (!this.grid.Cells.ContainsKey(bottomRight + Vector2.down)) //if (!neighbourCheckerBottomRight.neighbourDetected)
            {
                var entity = cellsPooler.GetEntity();
                var startPosition = new Vector2(transform.position.x + 0.5f, transform.position.y - 0.5f);
                entity.Initialize(this.cellsPooler, this, startPosition, Vector2.down, this.transform, true);
                AddCell(startPosition + Vector2.down, entity);
            }

            if (!this.grid.Cells.ContainsKey(topLeft + Vector2.left)) //if (!neighbourCheckerLeftTop.neighbourDetected)
            {
                var entity = cellsPooler.GetEntity();
                var startPosition = new Vector2(transform.position.x - 0.5f, transform.position.y + 0.5f);
                entity.Initialize(this.cellsPooler, this, startPosition, Vector2.left, this.transform, true);
                AddCell(startPosition + Vector2.left, entity);
            }

            if (!this.grid.Cells.ContainsKey(bottomLeft + Vector2.left)) //if (!neighbourCheckerLeftBottom.neighbourDetected)
            {
                var entity = cellsPooler.GetEntity();
                var startPosition = new Vector2(transform.position.x - 0.5f, transform.position.y - 0.5f);
                entity.Initialize(this.cellsPooler, this, startPosition, Vector2.left, this.transform, true);
                AddCell(startPosition + Vector2.left, entity);
            }

            if (!this.grid.Cells.ContainsKey(topRight + Vector2.right)) //if (!neighbourCheckerRightTop.neighbourDetected)
            {
                var entity = cellsPooler.GetEntity();
                var startPosition = new Vector2(transform.position.x + 0.5f, transform.position.y + 0.5f);
                entity.Initialize(this.cellsPooler, this, startPosition, Vector2.right, this.transform, true);
                AddCell(startPosition + Vector2.right, entity);
            }

            if (!this.grid.Cells.ContainsKey(bottomRight + Vector2.right)) //if (!neighbourCheckerRightBottom.neighbourDetected)
            {
                var entity = cellsPooler.GetEntity();
                var startPosition = new Vector2(transform.position.x + 0.5f, transform.position.y - 0.5f);
                entity.Initialize(this.cellsPooler, this, startPosition, Vector2.right, this.transform, true);
                AddCell(startPosition + Vector2.right, entity);
            }
		}
        
        private void OnParticleCollision(GameObject particleSystem)
        {
            if (particleSystem.tag.Contains(GameObjectTags.Flame))
            {
                RuntimeManager.PlayOneShot(this.DeathSound, this.transform.position);
                this.Disable();
                //IsActive = false;
                GetComponent<SpriteRenderer>().color = new Color32(247, 147, 30, 255);
            }
        }

        public bool ContainCellAtPosition(Vector2 position)
        {
            return this.grid.Cells.ContainsKey(position);
            //return this.cells.ContainsKey(position);
        }

        public bool AddCell(Vector2 position, CellSwarmBehavior cell)
        {
            if (this.grid.Cells.ContainsKey(position)) return false;

            Vector2 up = position + Vector2.up;
            Vector2 down = position + Vector2.down;
            Vector2 left = position + Vector2.left;
            Vector2 right = position + Vector2.right;

            if (this.cells.ContainsKey(up))
            {
                CellSwarmBehavior upCell = this.cells[up];
                upCell?.AddNeighbor(cell, position);
                cell.AddNeighbor(upCell, up);
            }
            if (this.cells.ContainsKey(down))
            {
                CellSwarmBehavior downCell = this.cells[down];
                downCell?.AddNeighbor(cell, position);
                cell.AddNeighbor(downCell, down);
            }
            if (this.cells.ContainsKey(left))
            {
                CellSwarmBehavior leftCell = this.cells[left];
                leftCell?.AddNeighbor(cell, position);
                cell.AddNeighbor(leftCell, left);
            }
            if (this.cells.ContainsKey(right))
            {
                CellSwarmBehavior rightCell = this.cells[right];
                rightCell?.AddNeighbor(cell, position);
                cell.AddNeighbor(rightCell, right);
            }
            cells.Add(position, cell);
            this.grid.Cells.Add(position, cell);

            RuntimeManager.PlayOneShot(this.GrowthSound, this.transform.position);
            return true;
        }

        public void UpdateCell(Vector2 position, CellSwarmBehavior cell) {
            if (cells.ContainsKey(position)){
                cells[position] = cell;
            }
            else{
                cells.Add(position,cell);
            }
        }

        public void RemoveCell(Vector2 position)
        {
            if (cells.ContainsKey(position))
            {
                foreach (CellSwarmBehavior neighbor in cells[position].Neighbors)
                {
                    neighbor?.RemoveNeighbor(cells[position], position);
                }
                cells.Remove(position);
                this.grid.Cells.Remove(position);
            }
        }

        //public void RemoveCell(CellSwarmBehavior cell)
        //{
        //    if (cells.ContainsValue(cell))
        //    {
        //        var itemsToRemove = (from pair in cells where pair.Value.Equals(cell) select pair.Key).ToList();
     
        //        foreach (var item in itemsToRemove)
        //        {
        //            foreach(CellSwarmBehavior neighbor in cell.Neighbors)
        //            {
        //                neighbor?.RemoveNeighbor(cell, item);
        //            }
        //            cells.Remove(item);
        //        }
        //    }
        //}

        //public CellSwarmBehavior[] GetNeighbors(Vector2 position)
        //{
        //    CellSwarmBehavior[] neighbors = new CellSwarmBehavior[4];
        //    Vector2 up = position + Vector2.up;
        //    neighbors[0] = this.GetCellFromPosition(up);
        //    Vector2 down = position + Vector2.down;
        //    neighbors[1] = this.GetCellFromPosition(down);
        //    Vector2 left = position + Vector2.left;
        //    neighbors[2] = this.GetCellFromPosition(left);
        //    Vector2 right = position + Vector2.right;
        //    neighbors[3] = this.GetCellFromPosition(right);

        //    return neighbors;
        //}

        //public CellSwarmBehavior GetCellFromPosition(Vector2 position) {
        //    if (cells.ContainsKey(position)){
        //        cells.TryGetValue(position, out var cell);
        //        return cell;
        //    }

        //    return null;
        //}

        protected override void OnDrawGizmosSelected()
        {
            Dictionary<Vector2, CellSwarmBehavior>.KeyCollection keys = this.cells.Keys;
            Gizmos.color = Color.black;
            foreach (Vector2 key in keys)
            {
                for(int i = 0; i < this.cells[key].Neighbors.Length; i++)
                {
                    if (this.cells[key].Neighbors[i] != null)
                    {
                        Vector2 direction = ((this.cells[key].Neighbors[i].transform.position - this.cells[key].transform.position).normalized * 0.25f);
                        Gizmos.DrawRay(this.cells[key].transform.position, direction);
                        //Gizmos.DrawLine(this.cells[key].transform.position, this.cells[key].Neighbors[i].transform.position);
                    }
                }
            }

            base.OnDrawGizmosSelected();
        }
        #endregion
    }
}
