using UnityEngine;

public static class DragManager
{
    public static GameObject dragIcon;      // Временная иконка перетаскиваемого предмета
    public static Equipment draggedItem;    // Предмет, который перетаскивают
    public static int sourceSlotIndex;        // Индекс исходного слота
    public static bool sourceIsInventory;     // true, если исходный слот из инвентаря, false – из панели быстрого доступа
}
