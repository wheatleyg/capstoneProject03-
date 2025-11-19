# Node MySQL 2.1

Lightweight, [mysql2][1]/[promise][2] based layer for easier database manipulation.

Inspired by PHP's awesome [dg/digi][3].

#### Method changes over mysql2

- [query](#query)
- [execute](#execute)
- [single](#single-result)
- [assoc](#associative-results)
- [pairs](#key-value-pairs)

------

## Code examples

#### Installation

```
npm -i mysql21
```

```js
// TypeScript
import { mysql21 } from 'mysql21';

// Node.js
const { mysql21 } = require("mysql21");
``` 

#### Connection

```js
const opts = {
  host: 'localhost',
  user: 'test',
  password: 'test',
  database: 'test',
  multipleStatements: true
}

// single line

let connection = await mysql21.createConnection(opts);

// pool

let connection = await mysql21.createPool(opts);
```

#### Query

```js
// either await

let query = await connection.query('SELECT 1');
console.log(query);
> [ TextRow { '1': 1 } ]

// or then...

connection.query('SELECT 1')
  .then(console.log);
> [ TextRow { '1': 1 } ]

connection.query('INSERT INTO listings (data) VALUES ?', [[[JSON.stringify({title: 'test'})]]])
  .then(console.log);
> 4
```

#### Execute

```js
connection.execute('SELECT ?', ['execute'])
  .then(console.log);
> execute
```

#### Single result

```js
connection.single('SELECT 1')
  .then(console.log);
> 1
```

#### Associative results

```js
connection.assoc('id', "SELECT 1 as id, 'one' as value UNION SELECT 2, 'two'")
      .then(console.log);
> {
>   '1': TextRow { id: 1, value: 'one' },
>   '2': TextRow { id: 2, value: 'two' }
> }
```

#### Key-Value Pairs

```js
connection.pairs('id', 'value', "SELECT 1 as id, 'one' as value UNION SELECT 2, 'two'")
      .then(console.log);
> { '1': 'one', '2': 'two' }
```

## API

Methods not described above follow their [mysql2][1] 
origin within it's [promise][2] wrapper.

Notice: `query()` and `execute()` drop field 
definitions from result for cleaner result parsing.

If you want them back, use `defQuery()` or
`defExecute()` instead, triggering mysql2's
original methods, which return `[result, fields]`.

## Error Handling

Simply `catch` the Errors as thrown by `mysql2`. 

They now have `sql` property of formatted string
for a chance of better debugging :-)

## Acknowledgements

  - Original @sidorares [mysql2][1] library
  - Awesome @dg [dibi][3] PHP library, I've been using for years  

[1]: https://www.npmjs.com/package/mysql2
[2]: https://www.npmjs.com/package/mysql2#using-promise-wrapper
[3]: https://github.com/dg/dibi

