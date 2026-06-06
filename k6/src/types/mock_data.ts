export interface UserData{
    UserId: number;
    Token: string;
    WorkspaceIds: number[];
    WorkspaceActions: WorkspaceAction[];
}

export interface WorkspaceAction{
    WorkspaceId: string;
    AddLearningItem?: LearningItem;
    UpdateLearningItem?: UpdateLearningItem;
    DeleteLearningItem?: DeleteLearningItem;
    CreateLearningItemConnection?: CreateLearningItemConnection;
    DeleteLearningItemConnection?: DeleteLearningItemConnection;
}

export interface LearningItem{
    id: string;
    title: string;
    description: string;
    status: string;
    baseVersion: number;
    idempotencyKey: string;
    type: string;
}

export interface UpdateLearningItem{
    id: string;
    description: string;
    baseVersion: number;
    idempotencyKey: string;
}

export interface DeleteLearningItem{
    id: string;
    incidentConnectionIds: string[];
    clientWorkspaceVersion: number;
    idempotencyKey: string;
}

export interface CreateLearningItemConnection{
    id: string;
    source: string;
    target: string;
    clientWorkspaceVersion: number;
    idempotencyKey: string;
}

export interface DeleteLearningItemConnection{
    id: string;
    clientWorkspaceVersion: number;
    idempotencyKey: string;
}
//     "Token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjMiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiRGFudGUgU3RhbnRvbiIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2VtYWlsYWRkcmVzcyI6IlNoeWFubi5HbG92ZXIxQHlhaG9vLmNvbSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlVzZXIiLCJleHAiOjE3ODQ2NDM5MDIsImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0OjcwNjYiLCJhdWQiOiJodHRwczovL2xvY2FsaG9zdDo3MDY2In0.F4ndIUTeBIaMudWefD-02f_hNyAkaRY-DM52Gw85TD8",
//     "WorkspaceIds": [
//       11,
//       2
//     ],
//     "WorkspaceActions": [
//       {
//         "WorkspaceId": "11",
//         "AddLearningItem": {
//           "id": "0b357b5ab6b846e887153d780d492c65",
//           "title": "Aut reiciendis sit.",
//           "description": "Necessitatibus facilis rerum incidunt ab dolores optio vero et tempore. Quis incidunt iusto voluptas dolor. Aspernatur fugiat esse voluptatum et qui et temporibus repellendus. Et facilis ut consequatur. Dolor quo minus nulla rerum ut.",
//           "status": "completed",
//           "baseVersion": 1,
//           "idempotencyKey": "4cd073d46e374b56be873dd5740d4ad2",
//           "type": "topic"
//         },
//         "UpdateLearningItem": {
//           "id": "6877e061dbe14dd2803897d13a9d31b6",
//           "description": "Sequi incidunt expedita voluptatibus fuga fugiat rerum similique reiciendis accusamus. Id molestiae repellendus fugiat. Minima ipsum enim qui aut. Eos impedit aut magni rerum.",
//           "baseVersion": 1,
//           "idempotencyKey": "d55adb56344f4c7dac1fd1ad9cbc6566"
//         },
//         "DeleteLearningItem": {
//           "id": "7c4f308c9fe94ad3a5899bc15f483fa6",
//           "incidentConnectionIds": [
//             "91c23da35e3f4b0ab228f819ae42716d-7c4f308c9fe94ad3a5899bc15f483fa6"
//           ],
//           "clientWorkspaceVersion": 1,
//           "idempotencyKey": "8158a91dc17f41de955848a8b0c8f2ba"
//         },
//         "CreateLearningItemConnection": {
//           "id": "2d65cd9050774a1faa6ce5059c710910",
//           "source": "ec1e8f02014a4ef88c64b579eaea91e7",
//           "target": "91c23da35e3f4b0ab228f819ae42716d",
//           "clientWorkspaceVersion": 1,
//           "idempotencyKey": "6c6f0aab34294cff92fd85f045f98e2c"
//         },
//         "DeleteLearningItemConnection": {
//           "id": "68da34d2e8a64668bfddf89d4f2249d8-d18a9b3702a44ac49b18545e5e7cd5c7",
//           "clientWorkspaceVersion": 1,
//           "idempotencyKey": "87fb023d552f441990acc3dbc783c9a9"
//         }
//       },
//       {
//         "WorkspaceId": "2",
//         "AddLearningItem": {
//           "id": "d38ff12824a643a7b98c30e9693f64e7",
//           "title": "Ipsa earum id.",
//           "description": "Aut aut assumenda voluptatibus nostrum reiciendis dolorem voluptatibus ex. Velit et voluptatem sit reiciendis tempora ad eos architecto. Maiores velit officia maxime commodi consequatur. Dolores in ipsam laborum nisi vel tenetur. Sit amet ipsa aut quas. Voluptatum nihil sed.",
//           "status": "inprogress",
//           "baseVersion": 26,
//           "idempotencyKey": "f0f65f7885d54f2d9abbe959fd7d926b",
//           "type": "subtopic"
//         }
//       }
//     ]
//   },