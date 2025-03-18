namespace DrillingCore.Core.Entities
{
    public class Equipment
    {
        public int Id { get; set; }

        // Например, Name — это название (или модель) оборудования
        public string Name { get; set; } = null!;

        // TypeId — ссылка на сущность EquipmentType (например, бурилка, водовоз и т.д.)
        public int TypeId { get; set; }

        // Регистрационный номер (номер машины и т.п.)
        public string RegistrationNumber { get; set; } = null!;

        // Дата создания записи
        public DateTime CreatedDate { get; set; }

        // Навигационное свойство на тип
        public virtual EquipmentType? EquipmentType { get; set; }
    }
}
