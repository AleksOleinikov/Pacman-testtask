using System.Collections.Generic;
using UnityEngine;

public abstract class RuntimeSet<T> : ScriptableObject
{
    [SerializeField] [TextArea(2, 10)] private string desctiption;
    public bool ClearOnStart;
    [Range(0,50)]public int MaxSize;    

    private void OnEnable()
    {
        if (ClearOnStart)
        {
            Debug.Log("Cleared: "+this.name);
            Clear();
        }
    }

    public List<T> Items = new List<T>();

    public void Add(T thing)
    {
        if (!Items.Contains(thing))
            Items.Add(thing);
    }

    public void Remove(T thing)
    {
        if (Items.Contains(thing))
            Items.Remove(thing);
    }

    public void Clear()
    {
        if (Items.Count != 0)
        {
            Items.Clear();
        }
    }

    ///<summary>Возвращает true/false если объект такого типа уже есть в Сэте</summary>
    public bool Exists(T obj)
    {        
        return Items.Exists(x => x.GetType() == obj.GetType());
    }

    ///<summary>Возвращает объект заданного типа из Сэта</summary>
    public T Find(T obj)
    {
        return Items.Find(x => x.GetType() == obj.GetType());
    }
}
