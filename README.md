# ECommerceApp
This is a full stack web application that uses Asp.Net Core 7 and an MVC pattern.
This a simple mock up of a shopping application.

Languages: C#, HTML, CSS, JS

CSS Framework: Bootstrap 5

Database: SQL server


## Features:

### Authentication:

Authentication is provided by Asp.Net Identity with added OAuth 2.0 Google account login. Users do not have a time-limited session.

### Shopping:

<img src="/ShoppingApp/ScreenShots/ProductPage.png" width="800" alt="Product Page">

Users are able to add products to their shopping cart. Items can both be added to the cart and removed. Users can search for products by name and/or category.

### Ordering:

<img src="/ShoppingApp/ScreenShots/Cart.png" width="800" alt="Shopping cart Page">

On the Checkout page, users can select a shipping address to use and finalize what items to purchase.

### Orders:

<img src="/ShoppingApp/ScreenShots/OrderPage.png" width="800" alt="Order Page">

This page houses the users past orders which can each be examined for further details.

### Database:

The database used is a Microsoft SQL server. Users, Orders, and Products are saved to the database.
