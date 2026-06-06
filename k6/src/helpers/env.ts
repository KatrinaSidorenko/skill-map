export const getEnv = () => {
    const apiBase = __ENV.API_BASE || 'https://localhost:7066';
    return {
        API_BASE: apiBase,
        MOCK_DATA_PATH: __ENV.MOCK_DATA_PATH || 'D:\\university-projects\\skill-map-tests\\load_testing\\mock_data.json',
        API_ROADMAPS_WORKSPACE: `${apiBase}/api/roadmaps-workspace`,
        WS_BASE_URL: __ENV.WS_BASE_URL || 'wss://localhost:7066',
    }
}