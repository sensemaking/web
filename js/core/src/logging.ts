let write = (entry : object) => console.log(entry)

export function configure(logger : (entry: object) => void) { write = logger }

export function log(entry : object) { write(entry) }

export function alert(code : string, message : string, error : Error, info : object) { 
    write({ code, message, exceptionDetail: error, additionalInfo: info  }) 
}