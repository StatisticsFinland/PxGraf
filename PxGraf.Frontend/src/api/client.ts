/* istanbul ignore file */

import { pxGrafUrl } from "utils/ApiHelpers"

interface IRequestParameters {
    method: string;
    headers: {
        [key: string]: string;
    };
    body?: string;
}

export class ApiClientV2 {

    BASE_URL_V2 = pxGrafUrl('api/');

    async postAsync(url: string, requestBody?: string) {

        const requestParameters: IRequestParameters = {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        }

        if (requestBody && requestBody.length > 0) {
            requestParameters.body = requestBody;
        }

        const response = await fetch(
            this.BASE_URL_V2 + url,
            requestParameters
        );

        if (!response.ok) {
            throw new Error('Network response error');
        }

        return response.json();
    }

    async getAsync(url: string, getParams?: { [key: string]: string }) {
        let getParamPart = '';
        if (getParams) {
            getParamPart = '?';
            Object.keys(getParams).forEach((key, index) => {
                getParamPart += key + '=';
                getParamPart += getParams[key];
                if (index + 1 <= Object.keys(getParams).length) getParamPart += '&';
            });
        }
        const response = await fetch(this.BASE_URL_V2 + url + getParamPart);

        if (!response.ok) {
            throw new Error('Network response error');
        }

        return response.json();
    }
}

export default ApiClientV2;

