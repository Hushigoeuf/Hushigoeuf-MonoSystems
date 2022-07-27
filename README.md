# Hushigoeuf-Extensions
 
Содержит классы для реализации расширений. Идея в том, чтобы можно было разделить код на части, которыми легко управлять и понимать. К примеру, eсли не добавить компонент PlayerHealth или удалить его, то персонаж перестанет получать урон. Это позволяет разделить сложные решения на части и управлять ими.

Например, создадим первый базовый класс Player, который будет отвечать за персонажа.

```csharp
public class Player : HGBaseExtension<PlayerExtension>
{
    public void TestMessage(string message)
    {
        Debug.Log(message);
    }
}

public abstract class PlayerExtension : HGExtension<Player>
{
}
```

И теперь мы можем сделать класс-расширениe для Player. Это тот самый класс PlayerHealth, который может обрабатывать столкновения и следить за здоровьем персонажа.

```csharp
public class PlayerHealth : PlayerExtension
{
}
```

Естественно, что и сам PlayerHealth может иметь свои расширения. Каждый такой класс имеет доступ к своему предшественнику.

```csharp
public class PlayerHealth : PlayerExtension
{
    public void Hit(float damage)
    {
        Base.TestMessage("Take hit: " + damage);
    }
}
```

Если использовать это в паре с любой системой событий, то можно избежать сильных зависимостей.

Пример использования такого решения:
https://github.com/Hushigoeuf/Hushigoeuf-SmashAllOfThem
