using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; 

public class MenuInputModule : BaseInputModule
{

    private bool m_ButtonHoldingDown = false;
    private float m_ButtonTimePressed = 0.0f;

    private GameObject m_CurrentFocusedGameObject;

    [SerializeField]
    private string m_InputJ1 = "MainMenuNavigatorJ1";

    [SerializeField]
    private string m_InputJ2 = "MainMenuNavigatorJ2";


    /// <summary>
    /// <para>Is the button pressed ?</para>
    /// </summary>
    public bool buttonHoldingDown
    {
        get
        {
            return this.m_ButtonHoldingDown;
        }
        set
        {
            this.m_ButtonHoldingDown = value;
        }
    }

    /// <summary>
    /// <para>The number of time the button is pressed.</para>
    /// </summary>
    public float buttonTimePressed
    {
        get
        {
            return this.m_ButtonTimePressed;
        }
        set
        {
            this.m_ButtonTimePressed = value;
        }
    }
    /// <summary>
    /// <para>Input manager name for the player J1 input.</para>
    /// </summary>
    public string inputJ1
    {
        get
        {
            return this.m_InputJ1;
        }
        set
        {
            this.m_InputJ1 = value;
        }
    }

    /// <summary>
    /// <para>Input manager name for the player J2 input.</para>
    /// </summary>
    public string inputJ2
    {
        get
        {
            return this.m_InputJ2;
        }
        set
        {
            this.m_InputJ2 = value;
        }
    }

    protected MenuInputModule()
    {
    }

    public override void ActivateModule()
    {
        if (base.eventSystem.isFocused)
        {
            base.ActivateModule();
            GameObject gameObject = base.eventSystem.currentSelectedGameObject;
            if(gameObject == null)
            {
                gameObject = base.eventSystem.firstSelectedGameObject;
            }
            base.eventSystem.SetSelectedGameObject(gameObject, this.GetBaseEventData());
        }
    }

    protected GameObject GeCurrentFocusedGameObject()
    {
        return this.m_CurrentFocusedGameObject;
    }

    public override void Process()
    { 
        if(base.eventSystem.isFocused)
        {
            bool selectedObject = SendUpdateEventToSelectedObject();
            if(base.eventSystem.sendNavigationEvents)
            {
                if(!selectedObject)
                {
                    selectedObject |= this.SendMoveEventToSelectedObject();
                }
                if(!selectedObject)
                {
                    this.SendSubmitEventToSelectedObject();
                }
            }
        }
        
    }

    protected bool SendSubmitEventToSelectedObject()
    {
        //bool flag;
        if(base.eventSystem.currentSelectedGameObject != null)
        {
            BaseEventData baseEventData = this.GetBaseEventData();
            if (base.input.GetButtonDown(this.m_InputJ1))
            {
                ExecuteEvents.Execute<ISubmitHandler>(base.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.submitHandler);
                buttonHoldingDown = true;
                m_ButtonTimePressed = 0f + Time.deltaTime;
            }
            if(Input.GetButtonUp(this.m_InputJ1) && buttonHoldingDown)
            {
                ExecuteEvents.Execute<ICancelHandler>(base.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.cancelHandler);
                buttonHoldingDown = false;
                m_ButtonTimePressed = 0f;
            }
        }
        return false;
    }

    protected bool SendUpdateEventToSelectedObject()
    {
        bool flag;
        if (base.eventSystem.currentSelectedGameObject != null)
        {
            BaseEventData baseEventData = this.GetBaseEventData();
            ExecuteEvents.Execute<IUpdateSelectedHandler>(base.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.updateSelectedHandler);
            flag = baseEventData.used;
        }
        else
        {
            flag = false;
        }
        return flag;
    }

    protected bool SendMoveEventToSelectedObject()
    {
        return false;
    }

}
