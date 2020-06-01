
using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using Game.Constants;
using UnityEngine;

public class NeighbourDetector : MonoBehaviour {

    #region Properties
    public bool neighbourDetected = false;
    private List<Collider2D> contacts = new List<Collider2D>();
    #endregion

    #region Methods

    public void Reset() {
        contacts.Clear();
        neighbourDetected = true;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag(GameObjectTags.Cell) || other.CompareTag(GameObjectTags.Wall) || other.CompareTag(GameObjectTags.Hearth)){
            contacts.Add(other);
        }

        neighbourDetected = contacts.Count > 0;
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag(GameObjectTags.Cell) || other.CompareTag(GameObjectTags.Wall) || other.CompareTag(GameObjectTags.Hearth)){
            contacts.Remove(other);
        }
        
        neighbourDetected = contacts.Count > 0;
    }

    private void LateUpdate() {
        var contactToRemove = new List<Collider2D>();
        foreach (var contact in contacts){
            if (!contact.gameObject.activeSelf){
                contactToRemove.Add(contact);
            }
        }

        foreach (var contact in contactToRemove){
            contacts.Remove(contact);
        }
        
        neighbourDetected = contacts.Count > 0;
    }

    public bool ContactAreWall() {
        var wall = 0;
        
        foreach (var contact in contacts){
            if (contact.CompareTag(GameObjectTags.Wall) && contact.gameObject.activeSelf){
                wall++;
            }
        }

        return wall > 0;
    }

    #endregion
}
