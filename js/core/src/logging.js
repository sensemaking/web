let adapter = {
    log() {
        console.log(...arguments)
    }
};

export function configure(logger) { adapter = logger }
export function log() { adapter.log(...arguments) }
export function alert(code, message, error, info) { adapter.log({ additionalInfo: info, exceptionDetail: error, code, message }) }