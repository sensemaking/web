import { configure, log, alert } from './logging'

describe(`Common Logging`, () => {
    test(`logging passes all arguments to adapter`, () => {        
        const adapter = { log: jest.fn() }
        configure(adapter)

        const toBeLogged = [`message 1`,`message 2`]        
        log(...toBeLogged)

        expect(adapter.log).toHaveBeenCalledWith(...toBeLogged)
    })

    test(`alerts are logged`, () => {
        const adapter = { log: jest.fn() }
        configure(adapter)

        const the_code = `0097`
        const the_message = `a message`
        const the_error = new Error(`Something went wrong`)
        const the_additional_info = { info: `something else` }
        alert(the_code, the_message, the_error, the_additional_info)
        
        expect(adapter.log).toHaveBeenCalledWith({ additionalInfo: the_additional_info, exceptionDetail: the_error, code: the_code, message: the_message })
    })
})
