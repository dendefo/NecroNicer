using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Face.FaceStats;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    int _options = 0;
    public int Options
    {
        get
        {
            return _options;
        }
        set
        {
            _options = value;
            Sort();
        }
    }

    int _filter = 0;
    public int Filter
    {
        get
        {
            return _filter;
        }
        set
        {
            _filter = value;
            Filtering();
        }
    }



    public List<InventoryItem> Inventory
    {
        get;
        private set;
    }
    [NonSerialized] public List<int> InventoryIndexes;

    public InventoryItem[] InventoryToCreate;

    [Header("UI")]
    public List<InventoryItemHandler> InventoryView;

    /// <summary>
    /// Page counter, goes in loops from minimal to maximum possible value
    /// </summary>
    int _currentPage = 0;
    public int CurrentPage
    {
        get { return _currentPage; }
        set
        {
            _currentPage = value;
            if (_currentPage < 0) _currentPage = InventoryIndexes.Count / InventoryView.Count;
            if (_currentPage > (InventoryIndexes.Count - 1) / InventoryView.Count) _currentPage = 0;
            CurrentPageText.text = (_currentPage + 1).ToString();
            Filtering();

        }
    }

    public TMPro.TMP_Text CurrentPageText;
    public TMPro.TMP_Text MaxPageText;
    private void Start()
    {
        Instance = this;
        Inventory = new List<InventoryItem>(InventoryToCreate);
        for (int i = 0; i < Inventory.Count; i++)
        {
            InventoryItem item = Inventory[i];
            item.Face.Uses = Mathf.Min(item.Face.Uses, item.Face.faceDefenition.Stats.MaxUses);
            Inventory[i] = item;
        }
        Sort();
    }

    public void InventoryAdd(InstanceFace face, int amount = 1)
    {
        bool sameFace(InventoryItem item)
        {
            return item.Face == face;
        }

        int index = Inventory.FindIndex(sameFace);
        if (index == -1)
        {
            Inventory.Add(new(face, amount));
            Filtering();
        }
        else
        {
            Inventory[index] = Inventory[index].Take(-amount);
            if (Inventory[index].Amount == 0)
            {
                Inventory.RemoveAt(index);
                if (Inventory.Count <= CurrentPage * InventoryView.Count)
                {
                    CurrentPage--;
                    MaxPageText.text = ((InventoryIndexes.Count - 1) / InventoryView.Count + 1).ToString();
                    return;
                }
                Filtering();
            }
            else
            {
                RefreshAll();

            }
        }
        

    }

    public void RefreshAll()
    {
        int b = 0;
        for (int i = 0; i < InventoryView.Count; i++)
        {
            if (i + (InventoryView.Count * CurrentPage) >= Inventory.Count)
            {
                Clean(i);
            }
            else Refresh(i);
        }
        MaxPageText.text = ((InventoryIndexes.Count - 1) / InventoryView.Count + 1).ToString();
    }

    public void Refresh(int number)
    {
        try
        {
            InventoryView[number].Item = Inventory[InventoryIndexes[number + (InventoryView.Count * CurrentPage)]];
        }
        catch { Clean(number); }
    }
    public void Clean(int number)
    {
        InventoryView[number].Item = new();
    }

    public void Sort()
    {
        switch (Options)
        {
            case 0: //Tier >
                Inventory.Sort(InventoryItem.CompareByTier);
                Inventory.Reverse();
                break;
            case 1: //Tier <
                Inventory.Sort(InventoryItem.CompareByTier);
                break;

            case 2: //Name >
                Inventory.Sort(InventoryItem.CompareByName);
                break;
            case 3: //Name <
                Inventory.Sort(InventoryItem.CompareByName);
                Inventory.Reverse();
                break;

            case 4: //Uses >
                Inventory.Sort(InventoryItem.CompareByDurability);
                Inventory.Reverse();
                break;
            case 5: //Uses <
                Inventory.Sort(InventoryItem.CompareByDurability);
                break;

            case 6: //Cost >
                Inventory.Sort(InventoryItem.CompareByCost);
                Inventory.Reverse();
                break;
            case 7: //Cost <
                Inventory.Sort(InventoryItem.CompareByCost);
                break;

            case 8: //Date >
                //Inventory.Sort(InventoryItem.CompareByDurability);
                break;
            case 9: //Date <
                //Inventory.Sort(InventoryItem.CompareByDice);
                break;
        }
        Filtering();

    }

    public void Filtering()
    {
        InventoryIndexes = new();
        int index = 0;
        Predicate<InventoryItem> filterer = x => true;

        switch (Filter)
        {
            case 0:
                filterer = x => true;
                break;
            case 1:
                filterer = x => x.Face.faceDefenition.Stats.ParentDice == DiceTypes.D8;
                break;
            case 2:
                filterer = x => x.Face.faceDefenition.Stats.ParentDice == DiceTypes.D10;
                break;
            case 3:
                filterer = face => face.Face.faceDefenition.Offensive;
                break;
            case 4:
                filterer = face => face.Face.faceDefenition.Defensive;
                break;
            case 5:
                filterer = face => face.Face.faceDefenition.Buff;
                break;
            case 6:
                filterer = face => face.Face.faceDefenition.Debuff;
                break;
        }

        for (int i = 0; i < Inventory.Count; i++)
        {
            index = Inventory.FindIndex(index, filterer);

            if (!InventoryIndexes.Contains(index)) InventoryIndexes.Add(index);
            index++;
        }
        try
        {
            if (InventoryIndexes[0] == -1) InventoryIndexes.Remove(-1);
        }
        catch { }
        RefreshAll();
    }
    public void ChangePage(int amount)
    {
        CurrentPage += amount;
    }
}

[Serializable]
public struct InventoryItem
{
    [Min(0)] [SerializeField] public int Amount;
    [SerializeField] public InstanceFace Face;
    public static int CompareByCost(InventoryItem x, InventoryItem y)
    {
        if (x.Face.faceDefenition.Stats.DustCost > y.Face.faceDefenition.Stats.DustCost) return 1;
        else if (x.Face.faceDefenition.Stats.DustCost == y.Face.faceDefenition.Stats.DustCost) return CompareByName(x, y);
        else return -1;
    }
    public static int CompareByName(InventoryItem x, InventoryItem y)
    {
        if (x.Face.faceDefenition.Stats.Name.CompareTo(y.Face.faceDefenition.Stats.Name) == 1) return 1;
        else if (x.Face.faceDefenition.Stats.Name.CompareTo(y.Face.faceDefenition.Stats.Name) == 0) return CompareByTier(x, y);
        else return -1;
    }
    public static int CompareByTier(InventoryItem x, InventoryItem y)
    {
        if (x.Face.faceDefenition.Stats.Tier > y.Face.faceDefenition.Stats.Tier) return 1;
        else if (x.Face.faceDefenition.Stats.Tier == y.Face.faceDefenition.Stats.Tier) return CompareByAmount(x, y);
        else return -1;
    }
    public static int CompareByAmount(InventoryItem x, InventoryItem y)
    {
        if (x.Amount > y.Amount) return 1;
        else if (x.Amount == y.Amount) return CompareByDurability(x, y);
        else return -1;
    }
    public static int CompareByDurability(InventoryItem x, InventoryItem y)
    {
        if (x.Face.Uses > y.Face.Uses) return 1;
        else if (x.Face.Uses == y.Face.Uses) return 0;
        else return -1;
    }

    public static bool EqualFace(InventoryItem left, InventoryItem right)
    {
        return (left.Face == right.Face);
    }
    public InventoryItem(InstanceFace face, int amount = 0)
    {
        Face = face;
        Amount = amount;
    }
    public InventoryItem Take(int amount)
    {
        return new(Face, Amount - amount);
    }
    public Material GetTexture()
    {
        if (Amount == 0) return null;
        return Face.faceDefenition.material;
    }
}