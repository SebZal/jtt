# jtt - JSON To Table

This program reads a JSON string from the console and prints it as a table.

Example usage:

```
echo '[{"name": "John", "age": 30}, {"name": "Jane", "age": 25}]' | jtt
```

```
┌───────┬─────┐
│ name  │ age │
├───────┼─────┤
│ John  │ 30  │
│ Jane  │ 25  │
└───────┴─────┘
```

It is also possible to transform data with the operations "find", "where", "select", "order by", "skip", and "take".

Example usage:

```
echo '[{"name": "John", "age": 30}, {"name": "Jane", "age": 25}]' | jtt find john
echo '[{"name": "John", "age": 30}, {"name": "Jane", "age": 25}]' | jtt where age gt 25
echo '[{"name": "John", "age": 30}, {"name": "Jane", "age": 25}]' | jtt select name
echo '[{"name": "John", "age": 30}, {"name": "Jane", "age": 25}]' | jtt order by age
echo '[{"name": "John", "age": 30}, {"name": "Jane", "age": 25}]' | jtt skip 1
echo '[{"name": "John", "age": 30}, {"name": "Jane", "age": 25}]' | jtt take 1
echo '[{"name": "John", "age": 30}, {"name": "Jane", "age": 25}]' | jtt find jane, order by age desc
```

You can also utilize CURL to fetch JSON data from an API and pipe it to the program.

Example usage:

```
curl -s 'https://jsonplaceholder.typicode.com/comments' | jtt select id postId email name, where postId eq 93

```

```
┌───────────────────────────────────────────────────────────────────────────────────────────────────────┐
│ id  │ postId │ email                 │ name                                                           │
├───────────────────────────────────────────────────────────────────────────────────────────────────────┤
│ 461 │ 93     │ Conner@daron.info     │ perferendis nobis praesentium accusantium culpa et et          │
│ 462 │ 93     │ Nathanael@jada.org    │ assumenda quia sint                                            │
│ 463 │ 93     │ Nicklaus@talon.io     │ cupiditate quidem corporis totam tenetur rem nesciunt et       │
│ 464 │ 93     │ Jerald@laura.io       │ quisquam quaerat rerum dolor asperiores doloremque             │
│ 465 │ 93     │ Jamey_Dare@johnny.org │ est sunt est nesciunt distinctio quaerat reprehenderit in vero │
└───────────────────────────────────────────────────────────────────────────────────────────────────────┘
```

Note that this doesn't cover any advanced scenarios like nested objects or complex queries.
It is just a simple implementation to cover the basic scenarios.

# Installation on Linux

Requires that you have installed the .NET SDK from https://dotnet.microsoft.com/download and also have Make installed on the system.

To install run

```sudo make install```

To uninstall run

```sudo make uninstall```
