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
     private WebDriverWait _wait;
#pragma warning restore NUnit1032 // An IDisposable field/property should be Disposed in a TearDown method


    [SetUp]
    public void Setup()
    {
        // Настройки 
        var options = new ChromeOptions();
        options.AddArguments("--start-maximized");
        _driver = new ChromeDriver(options);
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));
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

    private void OpenCommunity()
    {
        _driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/communities/c5c79346-8a50-447d-9fa8-0257e057acf0");
        _wait.Until(d => d.Url.Contains("c5c79346-8a50-447d-9fa8-0257e057acf0"));
       //Проверка, что находимся на нужной странице
        Assert.That(_driver.Url, Does.Contain("c5c79346-8a50-447d-9fa8-0257e057acf0"), "Не удалось перейти на страницу нужного сообщества");

    }

private void OpenPost()
{
     _driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/publications/b5c4f246-0a62-4d9a-b41c-6bc7ad3aa836");
    _wait.Until(d => d.Url.Contains("b5c4f246-0a62-4d9a-b41c-6bc7ad3aa836"));
    Assert.That(_driver.Url, Does.Contain("b5c4f246-0a62-4d9a-b41c-6bc7ad3aa836"), "Страница не открыта или URL не соответствует ожидаемому");
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
public void CommunityBuilding_Test()
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
    
    _wait.Until(d => d.Url.Contains("/communities/"));
    
    //Проверка создания 
   
 var titleSpan = _wait.Until(d => d.FindElement(By.CssSelector("span[data-tid='Title']")));
 var communityLink = titleSpan.FindElement(By.CssSelector("a.sc-eCApnc.fDWJqR"));
 Assert.That(communityLink.Text, Does.Contain("Автоматизация тестирования 2025"));

  //Переход в список сообществ
_driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/communities");

// Ожидание, пока страница загрузится и элементы станут доступными
_wait.Until(d => d.Title.Contains("Сообщества"));
Assert.That(_driver.Title,Does.Contain("Сообщества"));// Убедимся, что мы на нужной странице

// Использование метода
ScrollAndLoadCommunity(_driver, communityName);
var communityInList = _wait.Until(d => 
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

        // Ожидаем появления элемента с нужным названием
        var elements = driver.FindElements(By.XPath($"//*[contains(text(), '{communityName}')]"));

        if (elements.Count > 0)
        {
            // Если элементы найдены, выходим из цикла
            isLoaded = true;

            // Кликаем на первое найденное сообщество
            elements[0].Click();

            // Ожидаем появления элемента, подтверждающего загрузку сообщества
            _wait.Until(d => d.FindElement(By.XPath("//*[contains(text(), 'Пока новостей нет')]")));

            // Проверка, что элемент действительно есть
            var noNewsElement = driver.FindElement(By.XPath("//*[contains(text(), 'Пока новостей нет')]"));
            Assert.That(noNewsElement.Displayed, Is.True, "Элемент 'Пока новостей нет' не отображается");
        }
        else
        {
            // Ждем немного перед следующей прокруткой
            var jsExecutor = (IJavaScriptExecutor)driver;
            jsExecutor.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
        }
    }
}
 DeleteCommunity(communityName); //Удаление сообщества
}

private void DeleteCommunity(string communityName)
{  // Нажатие на три точки для открытия меню
     var moreOptionsButton = _driver.FindElement(By.XPath("//*[@id='root']/section/section[2]/div/div/section/div[2]/div[2]/div[2]/div/span"));
    moreOptionsButton.Click();

    // Переход на страницу настроек сообщества
    var settingsButton = _wait.Until(d => d.FindElement(By.XPath("/html/body/div[2]/div/div/div/div/div/div/span[2]/span")));
    settingsButton.Click();

   // Ожидание загрузки страницы настроек
    _wait.Until(d => d.Url.Contains("settings")); 
    // Проверка
    Assert.That(_driver.Url, Does.Contain("settings"), "Страница настроек не загрузилась, URL не содержит 'settings'");

    // Ожидание, пока кнопка "Удалить сообщество" станет доступной
    var deleteButton = _wait.Until(d => d.FindElement(By.XPath("//button[contains(., 'Удалить сообщество')]")));
    deleteButton.Click();

    // Подтверждение удаления
    var confirmButton = _wait.Until(d => d.FindElement(By.XPath("/html/body/div[3]/div/div[2]/div/div/div/div/div[3]/div[3]/div/div/div/div/div/div/span[1]/span/button")));
    confirmButton.Click();

    // Ожидание, пока сообщество будет удалено
    _wait.Until(d => d.FindElements(By.XPath($"//*[contains(text(), '{communityName}')]")).Count == 0);
  // Проверка, что сообщество действительно удалено
    VerifyCommunityDeletion(communityName);
}

private void VerifyCommunityDeletion(string communityName)
{
    // Переход в список сообществ
    _driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/communities");

    // Ожидание, пока страница загрузится и элементы станут доступными
    
    _wait.Until(d => d.Url.Contains("/communities"));

    // Проверка
    Assert.That(_driver.Url, Does.Contain("/communities"), "Страница со списком сообществ не загрузилась или URL не соответствует");

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
        
    _wait.Until(d =>
        {
            try
            {
                return true;
            }
            catch
            {
                return false;
            }
    });

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
   public void  CreatingPost_in_the_community_Test()
   
        {
            // Авторизация
            Authorize();
           
            // Запуск сообщества
           OpenCommunity();
            
            // Нажатие на элемент 
            var messageContainer = _wait.Until(d => d.FindElement(By.XPath("//*[@data-tid='AddButton']"))); 
            messageContainer.Click(); 

            // 4. Ожидание загрузки

            var commentField = _wait.Until(d => d.FindElement(By.XPath("//*[@id='root']/section/section[2]/div/div/div[1]/section/span/span/div/div/div[2]/div/div/div/div/div/div/span")));
             // Использование JavaScript для клика по полю
             IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
             js.ExecuteScript("arguments[0].scrollIntoView(true);", commentField); // Прокрутка к элементу
             js.ExecuteScript("arguments[0].click();", commentField); // Клик
        
            // 5. Ввод текста 
            commentField.SendKeys("Это тестовый пост.");

            // 6. Отправить
            
           var submitButton = _wait.Until(d => d.FindElement(By.XPath("//*[@id='root']/section/section[2]/div/div/div[1]/section/span/div/div/div[1]/div[2]/div/span[1]/span/button"))); // Замените на актуальный XPath для кнопки отправки
           submitButton.Click();

            // 7. Ожидание, чтобы убедиться, что комментарий был добавлен (опционально)
         _wait.Until(d => d.FindElement(By.XPath("//*[contains(text(), 'Это тестовый пост.')]")));
            
            //Проверка появления комментария 
          var commentElement = _wait.Until(d => d.FindElement(By.XPath("//*[contains(text(), 'Это тестовый пост.')]"))); 
          Assert.That(commentElement, Is.Not.Null, "Комментарий не найден.");  
        }
        
[Test]


public void DeletingPost_in_community_Test() // Удадение поста
{
    // Авторизация
    Authorize();

    // Запуск сообщества
    OpenCommunity();

    // Поиск комментария по тексту
  
    var commentElement = _wait.Until(d => d.FindElement(By.XPath("//*[contains(text(), 'Это тестовый пост.')]"))); 

    // 3. Нажатие на три точки для открытия меню
    var moreOptionsButton = commentElement.FindElement(By.XPath("//*[@id='root']/section/section[2]/div/div/div[2]/div/div/div/section/div[2]/div/div/div/div[1]/div[2]/div/div/div/span/button")); 
    moreOptionsButton.Click();
    //Проверка открытия меню
    var menuElement = new WebDriverWait(_driver, TimeSpan.FromSeconds(10)).Until(d => d.FindElement(By.XPath("//*[contains(@class, 'Это тестовый пост.')]")));
    Assert.That(menuElement.Displayed, Is.True, "Меню не открылось или не отображается");
    
    // 4. Нажатие на кнопку удаления комментария
    IWebElement deleteRecordButton = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//span[contains(text(), 'Удалить запись')]")));
    deleteRecordButton.Click();

    // 5. Подтверждение удаления 
    IWebElement element = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector(".react-ui-j884du")));
    element.Click();

    // 6. Ожидание
    _wait.Until(d => d.FindElements(By.XPath("//*[contains(text(), 'Это тестовый пост.')]")).Count == 0); 

    // Проверка пост удален
    var postElements = _driver.FindElements(By.XPath("//*[contains(text(), 'Это тестовый пост.')]"));
    Assert.That(postElements.Count, Is.EqualTo(0), "Пост все еще отображается на странице после удаления");
}

[Test]

// Создание комментария
public void CreatingComment_in_the_communityTest()
{
    // Авторизация
    Authorize();

    OpenPost();
  // Нажать кнопку "Комментировать"
   
   var commentElementButton = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector("button[data-tid='AddComment']")));
   commentElementButton.Click();
    
    // 3. Написать комментарий
    var commentBox = _driver.FindElement(By.ClassName("react-ui-r3t2bi"));
    commentBox.Clear(); // Очищаем поле, если необходимо
    commentBox.SendKeys("Уникальный комментарий"); 
   // Нажать "Отправить"
    var submitButton = _driver.FindElement(By.CssSelector("button.react-ui-m0adju"));
    submitButton.Click();
//Проверка
    bool commentAppeared = _wait.Until(d => d.PageSource.Contains("Уникальный комментарий"));
    Assert.That(commentAppeared, Is.True, "Комментарий не был добавлен или не отображается на странице");
    
}
[Test]

// Удаление комментария
  public void DeletingСomment_in_a_community_Test()
  
 {
    // Авторизация
    Authorize();

    // Открытие поста
    OpenPost();

    // 2. Найти уникальный комментарий
    var commentElement = _wait.Until(d => d.FindElement(By.XPath("//*[contains(text(), 'Уникальный комментарий')]")));

    // 3. Нажать кнопку удаления
    var removeCommentButton = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector("button[data-tid='RemoveComment']")));
    removeCommentButton.Click();
    Assert.That(removeCommentButton.Displayed, Is.True, "Кнопка корзины не отображается");
    Assert.That(removeCommentButton.Enabled, Is.True, "Кнопка  корзины неактивна");

    // 4. Подтвердить удаление
    var deleteButton = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector("span[data-tid='DeleteButton'] button.react-ui-aivml8")));
    deleteButton.Click();
    Assert.That(deleteButton.Displayed, Is.True, "Кнопка подтверждения удаления не отображается");
    Assert.That(deleteButton.Enabled, Is.True, "Кнопка подтверждения удаления неактивна");

    // 5. Проверка, что комментарий исчез
    bool commentAbsent = _wait.Until(d => !d.PageSource.Contains("Уникальный комментарий"));
    Assert.That(commentAbsent, Is.True, "Комментарий  присутствует после удаления");
 } 
 }


// Тесты 2 и 3, 4 и 5 между собой взаимосвязаны, зауск производить по очереди
