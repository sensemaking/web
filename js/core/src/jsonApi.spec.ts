import { describe, test, expect, beforeAll, afterAll, afterEach, beforeEach } from '@jest/globals'
import 'whatwg-fetch'
import { setupServer } from 'msw/node'
import { rest as mock, MockedRequest, MockedResponse } from 'msw'
import { get, put, post, del, ApiError } from './jsonApi'
import { HttpMethod, HttpStatusCode } from './http'

const noHandler = async (req: any, res: any, ctx: any) => {
    console.log(`No mocked handler setup for ${req.method} at ${req.url}`)
    return res(ctx.status(404))
}

const server = setupServer(mock.get(`*`, noHandler), mock.put(`*`, noHandler), mock.post(`*`, noHandler), mock.delete(`*`, noHandler))

beforeAll(() => server.listen())
afterEach(() => server.resetHandlers())
afterAll(() => server.close())

const url = `http://myapi.com/`
const payload = { wibble: `wobble` }

type CallDetail = { shouldSucceed: (req: MockedRequest) => boolean, wasCalled: boolean }
type Payload = { wibble: string }

const successfulWhen = (call: CallDetail)  => { call.wasCalled = false; return (req: any, res: any, ctx: any) => { 
    call.wasCalled = call.shouldSucceed(req)
    return res(ctx.status(200)) 
}}

const hasPayload = (payload: Payload) => (req: MockedRequest) => (req.body as Payload).wibble === payload.wibble
const isJson = (header: string) => (req: MockedRequest) => req.headers.get(header) === `application/json`

describe(`Methods`, () => {
    test(`GETs from a url`, async () => {
        const callDetail = { shouldSucceed: () => true, wasCalled: false }
        server.use(mock.get(url, successfulWhen(callDetail)))
        await get(url); return expect(callDetail.wasCalled).toBeTruthy()
    })

    test(`GET returns null when response has no content`, async () => {
        server.use(mock.get(url, (_, res, ctx) => res(ctx.status(200))))
        return expect(await get(url)).toBeNull()
    })

    test(`PUTs a body to a url`, async () => {
        const callDetail = { shouldSucceed: hasPayload(payload), wasCalled: false }
        server.use(mock.put(url, successfulWhen(callDetail)))
        await put(url, payload); return expect(callDetail.wasCalled).toBeTruthy()
    })

    test(`POSTs a body to a url`, async () => {
        const callDetail = { shouldSucceed: hasPayload(payload), wasCalled: false }
        server.use(mock.post(url, successfulWhen(callDetail)))
        await post(url, payload); return expect(callDetail.wasCalled).toBeTruthy()
    })

    test(`DELETEs from a url`, async () => {
        const callDetail = { shouldSucceed: () => true, wasCalled: false }
        server.use(mock.delete(url, successfulWhen(callDetail)))
        await del(url); return expect(callDetail.wasCalled).toBeTruthy()
    })
})

describe(`Headers`, () => {
    test(`All http requests add accept header for application/json`, async () => {
        const callDetail = { shouldSucceed: isJson(`Accept`), wasCalled: false }
        server.use(
            mock.get(url, successfulWhen(callDetail)),
            mock.put(url, successfulWhen(callDetail)),
            mock.post(url, successfulWhen(callDetail)),
            mock.delete(url, successfulWhen(callDetail))
        )
        
        await get(url); expect(callDetail.wasCalled).toBeTruthy()
        await put(url, {}); expect(callDetail.wasCalled).toBeTruthy()
        await post(url, {}); expect(callDetail.wasCalled).toBeTruthy()
        await del(url); return expect(callDetail.wasCalled).toBeTruthy()
    })

    test(`PUT & POST add content-type header for application/json`, async () => {
        const callDetail = { shouldSucceed: isJson(`Content-Type`), wasCalled: false }
        server.use(
            mock.put(url, successfulWhen(callDetail)),
            mock.post(url, successfulWhen(callDetail))
        )

        await put(url, {}); expect(callDetail.wasCalled).toBeTruthy()
        await post(url, {}); return expect(callDetail.wasCalled).toBeTruthy()
    })
})

describe(`Error Handling`, () => {
    const status = HttpStatusCode.InternalServerError
    const problem = { title: `Things ain't so good`, errors: [`What hasn't gone wrong`, `Catastophic rip in the space time continuum`] }

    beforeEach(() => server.use(mock.put(url, (_, res, ctx) => res(ctx.status(status), ctx.json(problem)))))

    test(`Errors provide the request url, method and payload`, async () => {
        return put(url, payload).catch((apiError: ApiError) => {
            expect(apiError.request.url).toBe(url)
            expect(apiError.request.method).toBe(HttpMethod.Put)
            return expect(apiError.request.payload).toBe(payload)
        })
    })

    test(`Errors provide the http status code and its text`, async () => {
        return put(url, payload).catch((apiError: ApiError) => {
            expect(apiError.status.code).toBe(status)
            return expect(apiError.status.text).toBe(HttpStatusCode[status])
        })
    })

    test(`Errors provide any problems detailed in the http response`, async () =>
        put(url, payload).catch((apiError: ApiError) => expect(apiError.problem).toStrictEqual(problem))
    )
})