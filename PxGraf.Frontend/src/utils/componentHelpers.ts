export const spacing = (s: number) => {
    return {
        /*
        Not first-child excluding style element.
        First-child is not safe on SSR because emotion can inject additional style elements.
        ...And even if no SSR is not used emotion will write console errors about it.
        */
        '*:not(style) + &': {
            mt: s,
        },
        '&:not(:last-child)': {
            mb: s,
        }
    };
}

export const a11yProps = (value: string | number) => {
    return {
        id: `simple-tab-${value}`,
        'aria-controls': `simple-tabpanel-${value}`,
    };
}
