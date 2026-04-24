/**
 * Enum for query publication status
 * @property {string} Unpublished - The query is unpublished.
 * @property {string} Success - The query was published successfully.
 * @property {string} Failed - The query publication failed.
 */
export enum EQueryPublicationStatus {
    Unpublished,
    Success,
    Failed
}