using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestFriendshipHolder : MonoBehaviour
{
    public Friendship friendship { get; private set; }
    public Button button;

    public Action<Friendship> onButtonPress;

    private void Start()
    {
        if (button == null)
            button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonPress);
    }

    public void SetFriendship(Friendship f)
    {
        this.friendship = f;
    }

    private void OnButtonPress()
    {
        onButtonPress?.Invoke(this.friendship);
    }
}
