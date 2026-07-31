/**
 * URL path builders for the application routes.
 * Kept in a standalone module to avoid circular dependencies between
 * Router.tsx (which imports Layout) and components that need these helpers.
 */
export const urls = {
    editor: (path: string[]) => {
        return `/editor/${path.join("/")}/`;
    },
    tableTree: `/`,
};
