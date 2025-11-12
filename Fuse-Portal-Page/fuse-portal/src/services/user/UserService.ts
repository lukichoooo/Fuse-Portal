
const enabledRoutes = ['/portal', '/uni-portal', '/career']

export default {
    isEnabled(route: string) {
        return enabledRoutes.includes(route)
    },
}

