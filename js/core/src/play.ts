interface Dog {
    walk: Function,
    age: number,
    bite: Function
}

interface Collie extends Dog {
    walk: Function,
    age: number,
    bite: Function
}

type A = Omit<Dog, Extract<keyof Dog, 'age' | 'chicken'>>


interface Todo {
    title: string,
    description: string,
    completed: string
}

type TodoPreview = Pick<Todo,'completed'>


class Weasel {
    constructor() {
            
    }

    get name() {
        return 'Weasel'
    }
}

function WorkWithWeasels(weasel : InstanceType<typeof Weasel>) {
    console.log(weasel.name);
}

WorkWithWeasels(new  Weasel())

WorkWithWeasels({ name: 'bob' })
