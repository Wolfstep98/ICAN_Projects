using System.Xml;
using Game.Constants;
using Game.Enums;
using Game.Serialization;
using UnityEngine;
using Pathfinding;
using NodeCanvas.Framework;

namespace Game.Entities.Ennemies
{
    [AddComponentMenu("Game/Enemies/Zombie")]
    public class ZombieEnemy : AbstractEnemy
    {
        #region Fields
        protected const string XmlMoveSpeed = "MoveSpeed";
        protected const string XmlTimeBeforeInvincible = "TimeBeforeInvincible";

        protected bool isInLight = false;
        [Header("Data")]
        [SerializeField] protected float moveSpeed = 2.0f;
        [SerializeField] private ParticleSystem flame2;
        #endregion

        #region Init
        public override void Initialize()
        {
            base.Initialize();

            this.lerp.speed = this.moveSpeed;
            this.lerp.destination = this.transform.position;
        }

        public virtual void Initialize(GameObject player)
        {
            this.blackboard.SetValue("playerTransform", player.transform);
            this.blackboard.SetValue("playerGameObject", player);
        }
        #endregion

        #region Properties
        public bool IsInLight { get { return this.isInLight; } }
        #endregion

        #region Methods
        //public virtual void InLightEnter()
        //{
        //    this.isInLight = true;
        //}
        //public virtual void InLightExit()
        //{
        //    this.isInLight = false;
        //}

        public override void TakeDamage(int amount, DamageSource source)
        {
            //if (this.isInLight) return;
            base.TakeDamage(amount, source);
        }

        protected override void Burn()
        {
            base.Burn();
            this.flame2.Play();
        }

        #region Serialization
        [ContextMenu("SaveXmlData", false)]
        public override void SaveXmlData()
        {
            XmlDocument document = XmlUtility.CreateDocument();

            XmlNode root = this.Serialize(document);
            document.AppendChild(root);

            string path = System.IO.Path.Combine(Application.streamingAssetsPath, XmlUtility.XmlEnemySavePath, XmlSaveFileName + "." + XmlUtility.Extension);
            document.Save(path);
        }

        public override XmlNode Serialize(XmlDocument document)
        {
            XmlNode rootNode = base.Serialize(document);
            {
                XmlNode moveSpeedNode = document.CreateElement(XmlMoveSpeed);
                {
                    moveSpeedNode.InnerText = this.moveSpeed.ToString();
                }
                rootNode.AppendChild(moveSpeedNode);

                //XmlNode timebeforeInvincibleNode = document.CreateElement(XmlTimeBeforeInvincible);
                //{
                //    timebeforeInvincibleNode.InnerText = this.timeBeforeInvincible.ToString();
                //}
                //rootNode.AppendChild(timebeforeInvincibleNode);
            }
            return rootNode;
        }

        protected override void LoadXmlData()
        {
            XmlDocument document = new XmlDocument();

            string path = System.IO.Path.Combine(Application.streamingAssetsPath, XmlUtility.XmlEnemySavePath, XmlSaveFileName + "." + XmlUtility.Extension);
            document.Load(path);

            XmlNode root = document.SelectSingleNode(XmlRoot);
            this.Deserialize(root);
        }

        public override void Deserialize(XmlNode rootNode)
        {
            base.Deserialize(rootNode);
            XmlNode moveSpeedNode = rootNode.SelectSingleNode(XmlMoveSpeed);
            {
                this.moveSpeed = float.Parse(moveSpeedNode.InnerText);
            }

            //XmlNode timebeforeInvincibleNode = rootNode.SelectSingleNode(XmlTimeBeforeInvincible);
            //{
            //    this.timeBeforeInvincible = float.Parse(timebeforeInvincibleNode.InnerText);
            //}
        }
        #endregion

        #region Collisions
        protected override void OnParticleCollision(GameObject particleSystem)
        {
            base.OnParticleCollision(particleSystem);

            if (particleSystem.tag.Contains(GameObjectTags.Flame))
            {
                this.TakeDamage(1, DamageSource.Fire);
            }
        }
        #endregion
        #endregion
    }
}