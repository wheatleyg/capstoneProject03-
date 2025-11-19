// TODO decide on TS or C#...
// Mr. Lemke please spare me but I'd probably do better with Node.JS than C# since I already have a ton of experience with JS/TS. 

//TS test to see if it works on my system :P



import { createPool, PoolOptions} from 'mysql2';



const access : PoolOptions = {
    host: 'localhost',
    user: 'root',
    password: 'PoofBall#1', //Don't do this in actual api, ja.
    database: 'fun_facts_db',
    waitForConnections: true,
    connectionLimit: 10,
    queueLimit: 0
    
};

export const connection = createPool(access);

connection.getConnection((err, conn) => {
    if (err) {
        console.error('Error you idiot!');
        return;
    }
    console.log('It worked!');
    conn.release();
})