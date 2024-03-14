Тестовое задание для души и работодателей.

Создание сайта каталога фильмов, с модерацией и привелениями при авторизации. Пока цель абстрактная, но со временем будет конкретеризоваться и дополняться.
Проект на Asp.Net Core MVC с использованием Entity Fraemwork Core и Identity. 

4 группы пользователей разделенных авторизацией и ролями.

Группы:

1 группа. Те кто просто зашел посмотреть каталог. Показывается все видео, которые прошли модерацию (далее с возможностью фильтрации). При просмотре подробнее фильма - не дается дополнительных функций (закрыт показ видео).

2 группа. Пользователи прошедшие регистрацию и авторизацию. Возможность добавлять описание к видео, редактировать/удалять - если не успели пройти процесс модерации. В каталоге отображается добавленные этим пользователем фильмы и прошедшие модерацию. Возможность смотреть видео.

3 группа. Модератор - человек (группа лиц), получившие права на проверку и допуск видео, добавленные пользователеми. Редактирует список жанров, стран, всех видео. При успешной проверке делает пост общедоступным (при этом возможность редактирования у пользователя пропадает). Имеет возможность дейсвовать как пользователь.

4 группа. Администратор - царь и Бог сайта. Возможности тонкой настройки - добавление/удаление ролей админа/модератора, настройка отправки сообщений на емайл (и еще что-нибудь пока не придумал).

Думаю еще добавить 1) что-нибудь что выполняется по расписанию 2) смену языка.

При первом запуске необходимо зайти под учеткой админа для создания роли админа.

Емайл и пароль админа - example@gmail.com / !QAZ2wsx
