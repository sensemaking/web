import { describe, test, expect, beforeAll, afterAll, afterEach, beforeEach } from '@jest/globals'
import 'whatwg-fetch'
import { setupServer } from 'msw/node'
import { rest as mock, MockedRequest } from 'msw'
import { get, put, post, del, ApiError } from './api'
import { HttpMethod, HttpStatusCode } from './http'

const noHandler = async (req: any, res: any, ctx: any) => {
    console.log(`No mocked handler setup for ${req.method} at ${req.url}`)
    return res(ctx.status(404))
}

const server = setupServer(mock.get(`*`, noHandler), mock.put(`*`, noHandler), mock.post(`*`, noHandler), mock.delete(`*`, noHandler))

beforeAll(() => server.listen())
afterEach(() => server.resetHandlers())
afterAll(() => server.close())

interface Payload { wibble: string }

const url = `http://myapi.com/`
const payload = { wibble: `wobble` }
const success = { success: true }
const failure = { success: false }

const successfulWhen = (predicate: Function) => (req: MockedRequest, res: any, ctx: any) => res(ctx.status(200), ctx.json(predicate(req) ? success : failure))
const hasPayload = (payload: Payload) => (req: MockedRequest) => (req.body as Payload).wibble === payload.wibble
const isJson = (header: string) => (req: MockedRequest) => req.headers.get(header) === `application/json`

describe(`Methods`, () => {
    test(`GETs from a url`, async () => {
        server.use(mock.get(url, successfulWhen(() => true)))
        return expect(await get(url)).toStrictEqual(success)
    })

    test(`PUTs a body to a url`, async () => {
        server.use(mock.put(url, successfulWhen(hasPayload(payload))))
        return expect(await put(url, payload)).toStrictEqual(success)
    })

    test(`POSTs a body to a url`, async () => {
        server.use(mock.post(url, successfulWhen(hasPayload(payload))))
        return expect(await post(url, payload)).toStrictEqual(success)
    })

    test(`DELETEs from a url`, async () => {
        server.use(mock.delete(url, successfulWhen(() => true)))
        return expect(await del(url)).toStrictEqual(success)
    })

    test(`Returns null when response has no content`, async () => {
        server.use(mock.get(url, (_, res, ctx) => res(ctx.status(200))))
        return expect(await get(url)).toBeNull()
    })
})

describe(`Headers`, () => {
    test(`All http requests add accept header for application/json`, async () => {
        server.use(
            mock.get(url, successfulWhen(isJson(`Accept`))),
            mock.put(url, successfulWhen(isJson(`Accept`))),
            mock.post(url, successfulWhen(isJson(`Accept`))),
            mock.delete(url, successfulWhen(isJson(`Accept`)))
        )

        expect(await get(url)).toStrictEqual(success)
        expect(await put(url, {})).toStrictEqual(success)
        expect(await post(url, {})).toStrictEqual(success)
        return expect(await del(url)).toStrictEqual(success)
    })

    test(`PUT & POST add content-type header for application/json`, async () => {
        server.use(
            mock.put(url, successfulWhen(isJson(`Content-Type`))),
            mock.post(url, successfulWhen(isJson(`Content-Type`)))
        )

        expect(await put(url, {})).toStrictEqual(success)
        return expect(await post(url, {})).toStrictEqual(success)
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