using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using NUnit.Framework;

namespace _1_test;

public class Tests
{
#pragma warning disable NUnit1032 // An IDisposable field/property should be Disposed in a TearDown method
    private ChromeDriver _driver;
#pragma warning restore NUnit1032 // An IDisposable field/property should be Disposed in a TearDown method


    [SetUp]
    public void Setup()
    {
        // Настройки 
        var options = new ChromeOptions();
        options.AddArguments("--start-maximized");
        _driver = new ChromeDriver(options);
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
    }

    [TearDown]
    public void TearDown()
    {
        _driver.Quit(); // Закрытие браузера после каждого теста
    }

    private void Authorize()
    {
        _driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru");
        
        
        _driver.FindElement(By.Id("Username")).SendKeys("u.semkina@skbkontur.ru");
        _driver.FindElement(By.Id("Password")).SendKeys("1998juliyA.");
        _driver.FindElement(By.Name("button")).Click();

        // Ожидание успешной авторизации
        new WebDriverWait(_driver, TimeSpan.FromSeconds(5))
            .Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='Title']")));
    }

    [Test]
    public void Authorisation_test()
    {
        Authorize();
        Assert.That(_driver.Title, Does.Contain("Новости"));
    }

    //[Test]
    //public void Example_another_test()
    // {
     //        Authorize();
      // Другие действия после авторизации
     //        _driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/profile");
    //     Assert.That(_driver.Title, Does.Contain("Профиль"));
    // }


[Test]

//Добавление и удаление сообщества
public void Test1()
{
    // Авторизация
    Authorize();
    
    // Переход на страницу создания сообщества
    _driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/communities");
    
    // Создание уникального имени сообщества
    var communityName = "Автоматизация тестирования 2025";

        // Нажатие кнопки "Создать сообщество"

        _driver.FindElement(By.XPath("//button[contains(., 'СОЗДАТЬ')]")).Click();

    // Заполнение данных сообщества
    var nameInput = _driver.FindElement(By.ClassName("react-ui-seuwan"));
    nameInput.SendKeys(communityName);;
    
    var descriptionInput = _driver.FindElement(By.ClassName("react-ui-r3t2bi")); 
    descriptionInput.SendKeys("Сообщество для профессионалов и энтузиастов в области автоматизации тестирования программного обеспечения. Здесь мы делимся опытом, инструментами и методологиями для повышения качества и эффективности тестирования.");
    // Баг, не создает более 5 символов
    // Нажатие кнопки сохранения
    var saveButton = _driver.FindElement(By.ClassName("react-ui-1f3jmd3"));
    saveButton.Click();

    // Ожидание создания и перехода на страницу сообщества
    var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
    
    // Проверка 1: URL содержит "/communities/"
    wait.Until(d => d.Url.Contains("/communities/"));
    
    // // Переход в список сообществ
_driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/communities");

// Ожидание, пока страница загрузится и элементы станут доступными
var wait1 = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));
wait.Until(d => d.Url.Contains("/communities")); // Убедимся, что мы на нужной странице

// Использование метода
ScrollAndLoadCommunity(_driver, communityName);
var communityInList = wait.Until(d => 
{
  var elements = d.FindElements(By.XPath($"//*[contains(text(), '{communityName}')]"));
return elements.FirstOrDefault();
});

// Поиск созданного сообщества в списке
void ScrollAndLoadCommunity(IWebDriver driver, string communityName)
{
    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
    bool isLoaded = false;
    
    while (!isLoaded)
    {
        // Прокрутка вниз страницы
        js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
        
        // Ждем, пока элементы загрузятся
        var elements = driver.FindElements(By.XPath($"//*[contains(text(), '{communityName}')]"));
        
        if (elements.Count > 0)
        {
            // Если элементы найдены, выходим из цикла
            isLoaded = true;
            
            // Открываем первое найденное сообщество
            elements[0].Click(); // Кликаем на первое найденное сообщество

            // Ожидаем загрузку сообщества 
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10)); 
            wait.Until(d => d.FindElement(By.XPath("//*[contains(text(), 'Пока новостей нет')]"))); 
        }
        else
        {
            // Ждем некоторое время перед следующей прокруткой
            Thread.Sleep(2000); // Время ожидания в миллисекундах
        }
    }
}
 DeleteCommunity(communityName);
}

private void DeleteCommunity(string communityName)
{  // Нажатие на три точки для открытия меню
     var moreOptionsButton = _driver.FindElement(By.XPath("//*[@id='root']/section/section[2]/div/div/section/div[2]/div[2]/div[2]/div/span"));
    moreOptionsButton.Click();

    // Переход на страницу настроек сообщества
    var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
    var settingsButton = wait.Until(d => d.FindElement(By.XPath("/html/body/div[2]/div/div/div/div/div/div/span[2]/span")));
    settingsButton.Click();

   // Ожидание загрузки страницы настроек
    wait.Until(d => d.Url.Contains("settings")); 

    // Ожидание, пока кнопка "Удалить сообщество" станет доступной
    var deleteButton = wait.Until(d => d.FindElement(By.XPath("//button[contains(., 'Удалить сообщество')]")));
    deleteButton.Click();

    // Подтверждение удаления
    var confirmButton = wait.Until(d => d.FindElement(By.XPath("/html/body/div[3]/div/div[2]/div/div/div/div/div[3]/div[3]/div/div/div/div/div/div/span[1]/span/button")));
    confirmButton.Click();

    // Ожидание, пока сообщество будет удалено
    wait.Until(d => d.FindElements(By.XPath($"//*[contains(text(), '{communityName}')]")).Count == 0);
  // Проверка, что сообщество действительно удалено
    VerifyCommunityDeletion(communityName);
}

private void VerifyCommunityDeletion(string communityName)
{
    // Переход в список сообществ
    _driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/communities");

    // Ожидание, пока страница загрузится и элементы станут доступными
    var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
    wait.Until(d => d.Url.Contains("/communities"));

    // Прокрутка и проверка отсутствия сообщества
    bool isCommunityDeleted = ScrollAndCheckCommunityAbsence(_driver, communityName);

    // Утверждение, что сообщество было удалено
    Assert.That(isCommunityDeleted, Is.True, "Сообщество не было удалено успешно.");
}

  private bool ScrollAndCheckCommunityAbsence(IWebDriver driver, string communityName)
{
    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
    bool isCommunityFound = false;

    // Прокрутка вниз страницы, пока не достигнем конца или не найдем сообщество
    for (int i = 0; i < 10; i++) // Прокрутка 10 раз
    {
        // Прокрутка вниз страницы
        js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
        
        // Ждем, пока элементы загрузятся
        Thread.Sleep(2000); // Время ожидания в миллисекундах

        // Проверка на наличие сообщества
        var elements = driver.FindElements(By.XPath($"//*[contains(text(), '{communityName}')]"));
        if (elements.Count > 0)
        {
            isCommunityFound = true;
            break; // Если нашли, выходим из цикла
        }
    }
    return !isCommunityFound; // Возвращаем true, если сообщество не найдено
}

[Test]

// Создание поста
   public void Test2()
   
        {
            // Авторизация
            Authorize();

            // 1. Переход в существующее сообщество
            _driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/communities/c5c79346-8a50-447d-9fa8-0257e057acf0");
            
            // 2. Открытие формы создания поста
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(d => d.Url.Contains("c5c79346-8a50-447d-9fa8-0257e057acf0"));

            // 3. Нажатие на элемент 
            var messageContainer = wait.Until(d => d.FindElement(By.XPath("//*[@data-tid='AddButton']"))); 
            messageContainer.Click(); 

            // 4. Ожидание загрузки

            var commentField = wait.Until(d => d.FindElement(By.XPath("//*[@id='root']/section/section[2]/div/div/div[1]/section/span/span/div/div/div[2]/div/div/div/div/div/div/span")));
             // Использование JavaScript для клика по полю
             IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
             js.ExecuteScript("arguments[0].scrollIntoView(true);", commentField); // Прокрутка к элементу
             js.ExecuteScript("arguments[0].click();", commentField); // Клик
        
            // 5. Ввод текста комментария
            commentField.SendKeys("Это тестовый комментарий.");

            // 6. Отправить
            
          var submitButton = wait.Until(d => d.FindElement(By.XPath("//*[@id='root']/section/section[2]/div/div/div[1]/section/span/div/div/div[1]/div[2]/div/span[1]/span/button"))); // Замените на актуальный XPath для кнопки отправки
           submitButton.Click();

            // 7. Ожидание, чтобы убедиться, что комментарий был добавлен (опционально)
         wait.Until(d => d.FindElement(By.XPath("//*[contains(text(), 'Это тестовый комментарий.')]")));

         
          var commentElement = wait.Until(d => d.FindElement(By.XPath("//*[contains(text(), 'Это тестовый комментарий.')]"))); 
          Assert.That(commentElement, Is.Not.Null, "Комментарий не найден.");  
        }
        
[Test]

// Ужадение поста
public void Test3()
{
    // Авторизация
    Authorize();

    // 1. Переход в существующее сообщество
    _driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/communities/c5c79346-8a50-447d-9fa8-0257e057acf0");

    // Ожидание, пока страница загрузится
    var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));
    wait.Until(d => d.Url.Contains("c5c79346-8a50-447d-9fa8-0257e057acf0"));

    // 2. Поиск комментария по тексту
    var commentElement = wait.Until(d => d.FindElement(By.XPath("//*[contains(text(), 'Это тестовый комментарий.')]"))); 

    // 3. Нажатие на три точки для открытия меню
    var moreOptionsButton = commentElement.FindElement(By.XPath("//*[@id='root']/section/section[2]/div/div/div[2]/div/div/div/section/div[2]/div/div/div/div[1]/div[2]/div/div/div/span/button")); 
    moreOptionsButton.Click();

    // 4. Нажатие на кнопку удаления комментария
    IWebElement deleteRecordButton = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//span[contains(text(), 'Удалить запись')]")));
    deleteRecordButton.Click();

    // 5. Подтверждение удаления 
    IWebElement element = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector(".react-ui-j884du")));
    Thread.Sleep(500); 
    element.Click();

    // 6. Ожидание
    wait.Until(d => d.FindElements(By.XPath("//*[contains(text(), 'Это тестовый комментарий.')]")).Count == 0); 
}
[Test]

// Создание комментария
public void Test4()
{
    // Авторизация
    Authorize();

    // 1. Переход в существующий пост
    _driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/publications/4241971a-5589-4c25-b835-2851afa22a96");

    // Ожидание, пока страница загрузится
    var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));
    wait.Until(d => d.Url.Contains("4241971a-5589-4c25-b835-2851afa22a96"));
    
    // 2. Нажать кнопку "Комментировать"
    var commentElementButton = _driver.FindElement(By.CssSelector("button[data-tid='AddComment']")); 
    Thread.Sleep(500);
    commentElementButton.Click();
    // 3. Написать комментарий
    var commentBox = _driver.FindElement(By.ClassName("react-ui-r3t2bi"));
    commentBox.Clear(); // Очищаем поле, если необходимо
    commentBox.SendKeys("Уникальный комментарий"); 
   // 4. Нажать "Отправить"
    var submitButton = _driver.FindElement(By.CssSelector("button.react-ui-m0adju"));
    submitButton.Click();

    // Ожидаем, пока комментарий появится на странице
    WebDriverWait _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
    bool commentAdded = wait.Until(d => 
    d.PageSource.Contains("Уникальный комментарий")); // Проверка на наличие текста комментария

    // Проверка результата
    if (commentAdded)
    {
    Console.WriteLine("Комментарий успешно добавлен.");
    }
    else
    {
    Console.WriteLine("Комментарий не найден.");
    }
    
}
[Test]

// Удаление комментария
  public void Test5()
  
  {
    // Авторизация
    Authorize();

    // 1. Переход в существующий комментарий
    _driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/publications/4241971a-5589-4c25-b835-2851afa22a96");
  
   // Ожидание, пока страница загрузится
    var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));
    wait.Until(d => d.Url.Contains("4241971a-5589-4c25-b835-2851afa22a96"));
   
   // 2. Найти уникальный комментарий
   var commentElement1 = wait.Until(d => d.FindElement(By.XPath("//*[contains(text(), 'Уникальный комментарий')]"))); 

  // 3. Найти и нажать на корзину
  var removeCommentButton = _driver.FindElement(By.CssSelector("button[data-tid='RemoveComment']"));
  Thread.Sleep(500);
  removeCommentButton.Click();

  // 4. Подтвердить удаление
  var deleteButton = _driver.FindElement(By.CssSelector("span[data-tid='DeleteButton'] button.react-ui-aivml8"));
  Thread.Sleep(500);
  deleteButton.Click();

  // 5. Ожидаем, пока действие завершится 
    WebDriverWait _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

 // 6. Проверка на отсутствие комментария на странице
  string uniqueCommentText = "Уникальный комментарий"; 
  bool commentDeleted = wait.Until(d => !d.PageSource.Contains(uniqueCommentText));
  if (commentDeleted)
    {
      Console.WriteLine("Комментарий успешно удален.");
    }
      else
    {
      Console.WriteLine("Комментарий все еще присутствует.");
    }         

  }
}

// Тесты 2 и 3, 4 и 5 между собой взаимосвязаны, зауск производить по очереди