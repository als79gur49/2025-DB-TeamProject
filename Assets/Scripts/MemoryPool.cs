using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryPool<T> where T : Component
{
    private class PoolItem
    {
        public bool isActive;
        public T item;
    }

    private int increaseCount = 5;
    private int maxCount;
    private int activedCount;

    private T poolObjectPrefab;
    private List<PoolItem> poolItemList;

    public int MaxCount => maxCount;
    public int ActivedCount => activedCount;

    // �ܼ� �θ� ������Ʈ�� ���� ������Ʈ�� �־ ���� ���ؼ�
    private Transform folder;
    public MemoryPool(T poolObjectPrefab, Transform folder, int increaseCount) : this(poolObjectPrefab, folder)
    {
        this.increaseCount = increaseCount;
    }
    public MemoryPool(T poolObjectPrefab, Transform folder)
    {
        maxCount = 0;
        activedCount = 0;
        this.poolObjectPrefab = poolObjectPrefab;
        this.folder = folder;
        poolItemList = new List<PoolItem>();

        InstantiateObjects();
    }
    public void InstantiateObjects()
    {
        maxCount += increaseCount;

        for (int i = 0; i < increaseCount; ++i)
        {
            PoolItem poolItem = new PoolItem();

            poolItem.isActive = false;
            poolItem.item = GameObject.Instantiate(poolObjectPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            //Debug.Log("MemoryPool folder = " + folder?.name);
            if (folder != null)
            {
                poolItem.item.transform.SetParent(folder.transform, true);
            }
            poolItem.item.gameObject.SetActive(false);

            poolItemList.Add(poolItem);
        }
    }

    // ��� ������Ʈ �ı�
    public void DestroyObjects()
    {
        if (poolItemList == null)
        {
            return;
        }

        for (int i = 0; i < poolItemList.Count; ++i)
        {
            if (poolItemList[i].item != null)
            {
                GameObject.Destroy(poolItemList[i].item.gameObject);
            }
        }

        poolItemList.Clear();
    }

    // Ǯ���� ��� ������ ������Ʈ Ȱ��ȭ �� ��ȯ
    public T ActivatePoolItem()
    {
        if (poolItemList == null)
        {
            return null;
        }

        if (maxCount == activedCount)
        {
            InstantiateObjects();
        }

        for (int i = 0; i < poolItemList.Count; ++i)
        {
            PoolItem poolItem = poolItemList[i];

            if (!poolItem.isActive)
            {
                activedCount++;

                poolItem.isActive = true;
                poolItem.item.gameObject.SetActive(true);

                return poolItem.item;
            }
        }

        return null;
    }

    // Ư�� ������Ʈ ��Ȱ��ȭ
    public void DeactivatePoolItem(T removeItem)
    {
        if (poolItemList == null || removeItem == null)
        {
            return;
        }

        for (int i = 0; i < poolItemList.Count; ++i)
        {
            PoolItem poolItem = poolItemList[i];

            if (poolItem.item == removeItem)
            {
                activedCount--;

                poolItem.isActive = false;
                poolItem.item.gameObject.SetActive(false);

                return;
            }
        }
    }

    // Ȱ��ȭ�� ù ������Ʈ ��Ȱ��ȭ �� ��ȯ
    public T DeactivatePoolItem()
    {
        if (poolItemList == null)
        {
            return null;
        }

        for (int i = 0; i < poolItemList.Count; ++i)
        {
            PoolItem poolItem = poolItemList[i];

            if (poolItem.item.gameObject.activeSelf)
            {
                activedCount--;

                poolItem.isActive = false;
                poolItem.item.gameObject.SetActive(false);

                return poolItem.item;
            }
        }

        return null;
    }

    // ��� Ȱ��ȭ�� ������Ʈ ��Ȱ��ȭ
    public void DeactivateAllPoolItem()
    {
        if (poolItemList == null)
        {
            return;
        }

        for (int i = 0; i < poolItemList.Count; ++i)
        {
            PoolItem poolItem = poolItemList[i];

            if (poolItem.item != null && poolItem.isActive)
            {
                poolItem.isActive = false;
                poolItem.item.gameObject.SetActive(false);
            }
        }

        activedCount = 0;
    }
}
